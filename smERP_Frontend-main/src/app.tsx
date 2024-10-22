import 'src/global.css';
import { Router } from 'src/routes/sections';
import { useScrollToTop } from 'src/hooks/use-scroll-to-top';
import { ThemeProvider } from 'src/theme/theme-provider';
import { AuthProvider } from './contexts/AuthContext';
import { PopupProvider } from './contexts/PopupContext';
import { QueryClientProvider, QueryClient } from '@tanstack/react-query';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';

// ----------------------------------------------------------------------

export default function App() {
  useScrollToTop();

  const queryClient = new QueryClient()

  return (
    <ThemeProvider>
      <QueryClientProvider client={queryClient}>
        <AuthProvider>
          <PopupProvider>
            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <Router />
            </LocalizationProvider>
          </PopupProvider>
        </AuthProvider>
      </QueryClientProvider>
    </ThemeProvider>
  );
}
