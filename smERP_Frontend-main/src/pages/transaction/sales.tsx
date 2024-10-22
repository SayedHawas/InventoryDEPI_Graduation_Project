import { Helmet } from 'react-helmet-async';

import { CONFIG } from 'src/config-global';
import { SalesView } from 'src/sections/transaction/sales/view';

// ----------------------------------------------------------------------

export default function Page() {
  return (
    <>
      <Helmet>
        <title> {`Sale Transactions - ${CONFIG.appName}`}</title>
      </Helmet>

      <SalesView />
    </>
  );
}
