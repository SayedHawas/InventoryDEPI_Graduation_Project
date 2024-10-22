import { Helmet } from 'react-helmet-async';

import { CONFIG } from 'src/config-global';
import { BrandView } from 'src/sections/brand/view';

// ----------------------------------------------------------------------

export default function Page() {
  return (
    <>
      <Helmet>
        <title> {`Brands - ${CONFIG.appName}`}</title>
      </Helmet>

      <BrandView />
    </>
  );
}
