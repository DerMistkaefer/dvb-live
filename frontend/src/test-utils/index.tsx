import React from 'react'
import {render, RenderOptions} from '@testing-library/react'
import {QueryClient, QueryClientProvider} from "react-query";

const queryClient = new QueryClient();
const AllTheProviders: React.ComponentType<{children: React.ReactElement}> = ({ children }) => (
    <QueryClientProvider client={queryClient}>
        {children}
    </QueryClientProvider>
);

const customRender = (ui: React.ReactElement, options?: Omit<RenderOptions, 'queries'>) =>
    render(ui, { wrapper: AllTheProviders, ...options })

// re-export everything
export * from '@testing-library/react'

// override render method
export { customRender as render }
