import { Helmet } from 'react-helmet-async';

import { CONFIG } from 'src/config-global';

import { AttributeView } from 'src/sections/attribute/view';

// ----------------------------------------------------------------------

export default function Page() {
  return (
    <>
      <Helmet>
        <title> {`Attributes - ${CONFIG.appName}`}</title>
      </Helmet>

      <AttributeView />
    </>
  );
}
