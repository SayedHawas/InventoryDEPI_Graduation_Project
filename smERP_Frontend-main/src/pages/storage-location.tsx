import { Helmet } from 'react-helmet-async';

import { CONFIG } from 'src/config-global';

import { StorageLocationView } from 'src/sections/storage-location/view';

// ----------------------------------------------------------------------

export default function Page() {
  return (
    <>
      <Helmet>
        <title> {`Storage Location- ${CONFIG.appName}`}</title>
      </Helmet>

      <StorageLocationView />
    </>
  );
}
