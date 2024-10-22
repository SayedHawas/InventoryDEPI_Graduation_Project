import { Helmet } from 'react-helmet-async';

import { CONFIG } from 'src/config-global';
import { BranchView } from 'src/sections/branch/view';

// ----------------------------------------------------------------------

export default function Page() {
  return (
    <>
      <Helmet>
        <title> {`Branches - ${CONFIG.appName}`}</title>
      </Helmet>

      <BranchView />
    </>
  );
}
