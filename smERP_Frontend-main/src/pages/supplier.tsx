import { Helmet } from 'react-helmet-async';

import { CONFIG } from 'src/config-global';

import { SupplierView } from 'src/sections/supplier/view';

// ----------------------------------------------------------------------

export default function Page() {
  return (
    <>
      <Helmet>
        <title> {`Suppliers - ${CONFIG.appName}`}</title>
      </Helmet>

      <SupplierView />
    </>
  );
}
