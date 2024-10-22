import React, { useState, useCallback } from 'react';
import {
  TableRow,
  TableCell,
  Checkbox,
  IconButton,
  Popover,
  MenuList,
  MenuItem,
} from '@mui/material';
import { menuItemClasses } from '@mui/material/MenuItem';
import { Iconify } from 'src/components/iconify';
import { TableColumn } from 'src/services/types';
import { TableAction } from './generic-table';

interface GenericTableRowProps<T, C = any> {
  row: T;
  columns: TableColumn<T>[];
  selected: boolean;
  onSelectRow?: (event: React.ChangeEvent<HTMLInputElement>, checked: boolean, id: string) => void;
  getRowId: (row: T) => string;
  actions?: TableAction<T, C>[];
  actionContext?: C;
  expandable?: boolean;
  expanded?: boolean;
  onExpandClick?: () => void;
}

export function GenericTableRow<T, C = any>({
  row,
  columns,
  selected,
  onSelectRow,
  getRowId,
  actions,
  actionContext,
  expandable,
  expanded,
  onExpandClick,
}: GenericTableRowProps<T, C>) {
  const [openPopover, setOpenPopover] = useState<HTMLButtonElement | null>(null);

  const handleOpenPopover = useCallback((event: React.MouseEvent<HTMLButtonElement>) => {
    setOpenPopover(event.currentTarget);
  }, []);

  const handleClosePopover = useCallback(() => {
    setOpenPopover(null);
  }, []);

  return (
    <>
      <TableRow hover tabIndex={-1} role="checkbox" selected={selected}>
        {expandable && (
          <TableCell>
            <IconButton
              aria-label="expand row"
              size="small"
              onClick={onExpandClick}
            >
              {expanded ? <Iconify icon="mingcute:up-fill" /> : <Iconify icon="mingcute:down-fill" />}
            </IconButton>
          </TableCell>
        )}
        {onSelectRow && ( // Only render the checkbox if onSelectRow is provided
          <TableCell padding="checkbox">
            <Checkbox
              disableRipple
              checked={selected}
              onChange={(event) => onSelectRow(event, !selected, getRowId(row))}
            />
          </TableCell>
        )}
        {columns.map((column) => (
          <TableCell key={column.id as string} align={column.align}>
            {column.render ? column.render(row) : (row[column.id as keyof T] as React.ReactNode)}
          </TableCell>
        ))}
        {actions && (
          <TableCell align="right">
            <IconButton onClick={handleOpenPopover}>
              <Iconify icon="eva:more-vertical-fill" />
            </IconButton>
          </TableCell>
        )}
      </TableRow>
      {actions && (
        <Popover
          open={!!openPopover}
          anchorEl={openPopover}
          onClose={handleClosePopover}
          anchorOrigin={{ vertical: 'top', horizontal: 'left' }}
          transformOrigin={{ vertical: 'top', horizontal: 'right' }}
        >
          <MenuList
            disablePadding
            sx={{
              p: 0.5,
              gap: 0.5,
              minWidth: 140,
              width: 'fit-content',
              display: 'flex',
              flexDirection: 'column',
              [`& .${menuItemClasses.root}`]: {
                px: 1,
                gap: 2,
                borderRadius: 0.75,
                [`&.${menuItemClasses.selected}`]: { bgcolor: 'action.selected' },
              },
            }}
          >
            {actions.map((action, index) => (
              <MenuItem
                key={index}
                onClick={() => {
                  action.onClick(row, actionContext);
                  handleClosePopover();
                }}
                sx={action.sx}
              >
                {action.icon && <Iconify icon={action.icon} />}
                {action.label}
              </MenuItem>
            ))}
          </MenuList>
        </Popover>
      )}
    </>
  );
}