using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Serilog;

namespace DerMistkaefer.DvbLive.ProxyHttp.FreeProxySharp.FreeProxy
{
    /// <summary>
    /// https://free-proxy-list.net/ scanner
    /// </summary>
    public static class FreeProxyListNet
    {
        /// <summary>
        /// base URL
        /// </summary>
        public const string URL = "https://free-proxy-list.net/";

        /// <summary>
        /// parse list of all proxies
        /// </summary>
        public static async Task<IEnumerable<FreeProxyServer>> Parse()
        {
            // parse port number
            static int ParsePort(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return 0;

                return int.TryParse(str, out var result) ? result : 0;
            }

            // parse proxy type
            static FreeProxyTypes ParseType(string str)
            {
                if (string.IsNullOrEmpty(str))
                    return FreeProxyTypes.Unknown;

                return str.ToUpperInvariant() switch
                {
                    "ANONYMOUS" => FreeProxyTypes.Anonymous,
                    "ELITE PROXY" => FreeProxyTypes.Elite,
                    _ => FreeProxyTypes.Transparent
                };
            }

            using var client = new HttpClient();
            var html = await client.GetStringAsync(URL).ConfigureAwait(false);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var rowsTemp = doc.DocumentNode.QuerySelectorAll("table tbody tr").ToArray();
            var rows = rowsTemp.Select(p => new FreeProxyServer()
                {
                    Ip = p.QuerySelector("td:nth-child(1)")?.InnerHtml,
                    Port = ParsePort(p.QuerySelector("td:nth-child(2)")?.InnerHtml),
                    Code = p.QuerySelector("td:nth-child(3)")?.InnerHtml,
                    Country = p.QuerySelector("td:nth-child(4)")?.InnerHtml,
                    Type = ParseType(p.QuerySelector("td:nth-child(5)")?.InnerHtml),
                    IsHttps = p.QuerySelector("td:nth-child(7)")?.InnerHtml != "no",
                })
                .Where(x => x.Port > 0 && x.Type != FreeProxyTypes.Unknown)
                .ToArray();

            foreach (var row in rows)
            {
                Log.Verbose($"{row.Ip}:{row.Port} ({row.Code} - {row.Country}) {row.Type}");
            }

            return rows.ToArray();
        }

        /// <summary>
        /// check proxy list
        /// </summary>
        public static async Task<IEnumerable<FreeProxyServer>> Check(IEnumerable<FreeProxyServer> list,
            bool nonTransparentOnly = true, string[]? codeFilter = null, int required = 10, int maxMilliseconds = 1000,
            bool? https = true, int timeoutSeconds = 5)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            
            // for non-transparent forget this kind in input data
            if (nonTransparentOnly)
            {
                list = list.Where(x => x.Type != FreeProxyTypes.Transparent);
                Log.Debug($"Filter: [nonTransparentOnly] {list.Count()} proxies.");
            }

            // need support HTTPS?
            if (https != null)
            {
                list = list.Where(x => x.IsHttps == https);
                Log.Debug($"Filter: [https] {list.Count()} proxies.");
            }

            // filter by Country codes; when defined
            if (codeFilter?.Length > 0)
            {
                var _filter = codeFilter.Select(x => x.ToUpperInvariant()).ToArray();
                list = list.Where(x => _filter.Contains(x.Code.ToUpperInvariant()));
                Log.Debug($"Filter: [codeFilter] {list.Count()} proxies.");
            }

            Log.Debug($"Check: {list.Count()} proxies, {required} required.");

            var num = 0;
            var tasks = new List<Task<FreeProxyServer?>>();
            foreach (var p in list)
            {
                tasks.Add(CheckProxy(p, num, nonTransparentOnly, maxMilliseconds, timeoutSeconds));
                num++;
            }
            var result = await Task.WhenAll(tasks);
            
            /*// check if already has count of required
            if (required > 0 && result.Count >= required)
                break;*/

            return result
                .Where(x => x is not null)
                .OrderBy(x => x!.ElapsedMilliseconds)
                .Take(required)!;
        }

        private static async Task<FreeProxyServer?> CheckProxy(FreeProxyServer p, int num, bool nonTransparentOnly,
            int maxMilliseconds, int timeoutSeconds)
        {
            var label = $"#{num} {p.Ip}:{p.Port} {p.Note}";

            // create client with proxy
            using var handler = new HttpClientHandler() {Proxy = new WebProxy(p.Ip, p.Port), UseProxy = true,};
            using var client = new HttpClient(handler) {Timeout = new TimeSpan(0, 0, timeoutSeconds)};
            // check myself IP
            var ipValue = "";
            try
            {
                var watch = Stopwatch.StartNew();

                ipValue = await GetStringSafeAsync(client, "http://checkip.amazonaws.com/");
                ipValue = ipValue?.Replace("\n", "").Replace("\r", "");
                if (string.IsNullOrEmpty(ipValue))
                {
                    Log.Debug("{Label} [empty #1 - checkip]", label);
                    return null;
                }

                // save elapsed milliseconds
                watch.Stop();
                p.ElapsedMilliseconds = watch.ElapsedMilliseconds;
            }
            catch (HttpRequestException)
            {
                Log.Debug("{Label} [exception]", label);
                return null;
            }
            catch (TaskCanceledException)
            {
                Log.Debug("{Label} [task cancelled]", label);
                return null;
            }
            catch (OperationCanceledException)
            {
                Log.Debug("{Label} [operation cancelled]", label);
                return null;
            }

            // check if proxy is not transparent (visible IP is the same as proxy IP)
            if (nonTransparentOnly)
            {
                // check myself IP with proxy settings
                if (ipValue != p.Ip)
                {
                    Log.Debug($"{label} [ip]");
                    return null;
                }
            }

            // check latency
            if (maxMilliseconds > 0 && p.ElapsedMilliseconds > maxMilliseconds)
            {
                Log.Debug($"{label} [slow in {p.ElapsedMilliseconds}ms]");
                return null;
            }

            Log.Debug($"{label} [OK in {p.ElapsedMilliseconds}ms]");

            return p;
        }

        private static async Task<string?> GetStringSafeAsync(HttpClient client, string url)
        {
            try
            {
                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (TaskCanceledException)
            {
                Log.Warning("Retry [task cancelled] failed, url: '{Url}'", url);
                return null;
            }
            catch (OperationCanceledException)
            {
                Log.Warning("Retry [operation cancelled] failed, url: '{Url}'", url);
                return null;
            }
        }

        /// <summary>
        /// parse & check & assign to configuratuin proxies
        /// </summary>
        public static void CheckAndAssignToConfig(this IHttpProxyConfiguration configuration,
            bool nonTransparentOnly = true, string[] codeFilter = null, int required = 10, int maxMilliseconds = 1000,
            bool? https = true,
            bool throwWhenLessThanRequired = false)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var proxies = Parse().GetAwaiter().GetResult();
            var checkedProxies = Check(proxies, nonTransparentOnly, codeFilter, required, maxMilliseconds, https)
                .GetAwaiter().GetResult();

            if (checkedProxies.Count() < required && throwWhenLessThanRequired)
                throw new InvalidOperationException($"Found: {checkedProxies.Count()}, required: {required} proxies");

            configuration.Proxies = checkedProxies.ToArray();
        }
    }
}