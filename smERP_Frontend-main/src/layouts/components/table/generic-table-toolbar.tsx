import React, { ReactNode } from 'react';
import { Toolbar, OutlinedInput, InputAdornment, Box } from "@mui/material";
import { Iconify } from "src/components/iconify";
import { Scrollbar } from 'src/components/scrollbar';

interface GenericTableToolbarProps {
  numSelected: number;
  filterName: string;
  onFilterName: (event: React.ChangeEvent<HTMLInputElement>) => void;
  customFilters?: ReactNode;
}

export function GenericTableToolbar({
  numSelected,
  filterName,
  onFilterName,
  customFilters
}: GenericTableToolbarProps) {
  return (
    <Scrollbar>
      <Toolbar
        sx={{
          height: 'auto',
          minHeight: 96,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'flex-start',
          padding: (theme) => theme.spacing(2, 1, 2, 3),
          ...(numSelected > 0 && {
            color: 'primary.main',
            bgcolor: 'primary.lighter',
          }),
        }}
      >
        {numSelected > 0 ? (
          <div>{numSelected} selected</div>
        ) : (
          <>
            <Box sx={{ display: 'flex', width: 'auto', marginBottom: '16px', gap: 2 }}>
              <OutlinedInput
                value={filterName}
                onChange={onFilterName}
                placeholder="Search..."
                startAdornment={
                  <InputAdornment position="start">
                    <Iconify icon="eva:search-fill" sx={{ color: 'text.disabled', width: 20, height: 20 }} />
                  </InputAdornment>
                }
                sx={{ flex: 0, width: "auto", minWidth: "200px" }}
              />
              {customFilters}
            </Box>
          </>
        )}
      </Toolbar>
    </Scrollbar>
  );
}