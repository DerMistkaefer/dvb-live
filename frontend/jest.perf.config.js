//const baseConfig = require('./jest.config.js')
module.exports = {
    "setupFiles": [require.resolve('react-app-polyfill/jsdom')],
    testRunner: require.resolve('jest-circus/runner'),
    transform: {
        '^.+\\.(js|jsx|mjs|cjs|ts|tsx)$': require.resolve(
            './node_modules/react-scripts/config/jest/babelTransform.js'
        ),
        '^.+\\.css$': require.resolve('./node_modules/react-scripts/config/jest/cssTransform.js'),
        '^(?!.*\\.(js|jsx|mjs|cjs|ts|tsx|css|json)$)': require.resolve(
            './node_modules/react-scripts/config/jest/fileTransform.js'
        ),
    },
    'cacheDirectory': './node_modules/.jestCachePerf',
    'testMatch': ['**/?(*.)(spec|test?(s)).perf.ts?(x)'],
    "setupFilesAfterEnv": ["jest-performance"],
}