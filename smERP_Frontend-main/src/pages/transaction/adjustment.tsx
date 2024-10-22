import { Helmet } from 'react-helmet-async';

import { CONFIG } from 'src/config-global';
import { AdjustmentView } from 'src/sections/transaction/adjustment/view';

// ----------------------------------------------------------------------

export default function Page() {
  return (
    <>
      <Helmet>
        <title> {`Adjustment Transactions - ${CONFIG.appName}`}</title>
      </Helmet>

      <AdjustmentView />
    </>
  );
}
