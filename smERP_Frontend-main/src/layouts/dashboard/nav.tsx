import type { Theme, SxProps, Breakpoint } from '@mui/material/styles';

import { useEffect, useState } from 'react';

import Box from '@mui/material/Box';
import ListItem from '@mui/material/ListItem';
import { useTheme } from '@mui/material/styles';
import ListItemButton from '@mui/material/ListItemButton';
import Drawer, { drawerClasses } from '@mui/material/Drawer';

import { usePathname } from 'src/routes/hooks';
import { RouterLink } from 'src/routes/components';

import { varAlpha } from 'src/theme/styles';

import { Logo } from 'src/components/logo';
import { Scrollbar } from 'src/components/scrollbar';

import { WorkspacesPopover } from '../components/workspaces-popover';

import type { WorkspacesPopoverProps } from '../components/workspaces-popover';
import { Collapse } from '@mui/material';
import { Iconify } from 'src/components/iconify';
import { useAuth } from 'src/contexts/AuthContext';

// ----------------------------------------------------------------------

export type NavContentProps = {
  data: {
    path: string;
    title: string;
    icon: React.ReactNode;
    info?: React.ReactNode;
    roles?: string[]
  }[];
  slots?: {
    topArea?: React.ReactNode;
    bottomArea?: React.ReactNode;
  };
  workspaces: WorkspacesPopoverProps['data'];
  sx?: SxProps<Theme>;
};

export function NavDesktop({
  sx,
  data,
  slots,
  workspaces,
  layoutQuery,
}: NavContentProps & { layoutQuery: Breakpoint }) {
  const theme = useTheme();

  return (
    <Box
      sx={{
        pt: 2.5,
        px: 2.5,
        top: 0,
        left: 0,
        height: 1,
        display: 'none',
        position: 'fixed',
        flexDirection: 'column',
        bgcolor: 'var(--layout-nav-bg)',
        zIndex: 'var(--layout-nav-zIndex)',
        width: 'var(--layout-nav-vertical-width)',
        borderRight: `1px solid var(--layout-nav-border-color, ${varAlpha(theme.vars.palette.grey['500Channel'], 0.12)})`,
        [theme.breakpoints.up(layoutQuery)]: {
          display: 'flex',
        },
        ...sx,
      }}
    >
      <NavContent data={data} slots={slots} workspaces={workspaces} />
    </Box>
  );
}

// ----------------------------------------------------------------------

export function NavMobile({
  sx,
  data,
  open,
  slots,
  onClose,
  workspaces,
}: NavContentProps & { open: boolean; onClose: () => void }) {
  const pathname = usePathname();

  useEffect(() => {
    if (open) {
      onClose();
    }
  }, [pathname]);

  return (
    <Drawer
      open={open}
      onClose={onClose}
      sx={{
        [`& .${drawerClasses.paper}`]: {
          pt: 2.5,
          px: 2.5,
          overflow: 'unset',
          bgcolor: 'var(--layout-nav-bg)',
          width: 'var(--layout-nav-mobile-width)',
          ...sx,
        },
      }}
    >
      <NavContent data={data} slots={slots} workspaces={workspaces} />
    </Drawer>
  );
}

// ----------------------------------------------------------------------

export function NavContent({ data, slots, workspaces, sx }: NavContentProps) {
  const pathname = usePathname();
  const { user } = useAuth();

  const filteredData = data.filter(item => 
    !item.roles || (user && item.roles.some(role => user.roles.includes(role)))
  );

  return (
    <>
      <Logo />

      {slots?.topArea}

      {/* <WorkspacesPopover data={workspaces} sx={{ my: 2 }} /> */}

      <Scrollbar fillContent>
        <Box component="nav" display="flex" flex="1 1 auto" flexDirection="column" sx={sx}>
          <Box component="ul" gap={0.5} display="flex" sx={{ my: 2 }} flexDirection="column">
          {filteredData.map((item) => (
              <NavItem key={item.title} item={item} pathname={pathname} userRoles={user ? user.roles : []} />
            ))}
          </Box>
        </Box>
      </Scrollbar>

      {slots?.bottomArea}
    </>
  );
}

function NavItem({ item, pathname, userRoles }: { item: any; pathname: string; userRoles: string[] }) {
  const [open, setOpen] = useState(false);
  const isActived = item.path === pathname;
  const hasSubItems = item.subItems && item.subItems.length > 0;

  const handleClick = () => {
    if (hasSubItems) {
      setOpen(!open);
    }
  };

  const filteredSubItems = hasSubItems 
  ? item.subItems.filter((subItem: any) => 
      !subItem.roles || subItem.roles.some((role: any) => userRoles.includes(role))
    )
  : [];

  if (hasSubItems && filteredSubItems.length === 0) {
    return null;
  }

  return (
    <>
      <ListItem disableGutters disablePadding>
        <ListItemButton
          disableGutters
          component={hasSubItems ? 'div' : RouterLink}
          href={hasSubItems ? undefined : item.path}
          onClick={handleClick}
          sx={{
            pl: 2,
            py: 1,
            gap: 2,
            pr: 1.5,
            borderRadius: 0.75,
            typography: 'body2',
            fontWeight: 'fontWeightMedium',
            color: 'var(--layout-nav-item-color)',
            minHeight: 'var(--layout-nav-item-height)',
            ...(isActived && {
              fontWeight: 'fontWeightSemiBold',
              bgcolor: 'var(--layout-nav-item-active-bg)',
              color: 'var(--layout-nav-item-active-color)',
              '&:hover': {
                bgcolor: 'var(--layout-nav-item-hover-bg)',
              },
            }),
          }}
        >
          <Box component="span" sx={{ width: 24, height: 24 }}>
            {item.icon}
          </Box>

          <Box component="span" flexGrow={1}>
            {item.title}
          </Box>

          {item.info && item.info}

          {hasSubItems && (open ? <Iconify icon='mingcute:up-line'/> : <Iconify icon='mingcute:down-line'/> )}
        </ListItemButton>
      </ListItem>

      {hasSubItems && (
        <Collapse in={open} timeout="auto" unmountOnExit>
          <Box component="ul" sx={{ pl: 2 }}>
            {filteredSubItems.map((subItem: any) => (
              <NavItem key={subItem.title} item={subItem} pathname={pathname} userRoles={userRoles} />
            ))}
          </Box>
        </Collapse>
      )}
    </>
  );
}