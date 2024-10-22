import { Helmet } from 'react-helmet-async';

import { CONFIG } from 'src/config-global';
import { ProcurementView } from 'src/sections/transaction/procurement/view';

// ----------------------------------------------------------------------

export default function Page() {
  return (
    <>
      <Helmet>
        <title> {`Procurement Transactions - ${CONFIG.appName}`}</title>
      </Helmet>

      <ProcurementView />
    </>
  );
}
