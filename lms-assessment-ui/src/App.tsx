import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import Homepage from "./components/homepage";

const queryClient = new QueryClient();

export function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Homepage />
    </QueryClientProvider>
  );
}
