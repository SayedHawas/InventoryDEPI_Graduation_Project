import React, { ReactNode, useState } from 'react';
import {
  Table,
  TableBody,
  TableContainer,
  TablePagination,
  Card,
  Collapse,
  Box,
  TableCell,
  TableRow,
  SxProps,
  Theme,
} from '@mui/material';
import { TableColumn } from 'src/services/types';
import { GenericTableRow } from './generic-table-row';
import { GenericTableHead } from './generic-table-head';
import { GenericTableToolbar } from './generic-table-toolbar';
import { TableEmptyRows } from './generic-table-empty-row';
import { TableNoData } from './generic-table-no-date';
import { Scrollbar } from 'src/components/scrollbar';

interface GenericTableProps<T, C = any> {
  data: T[];
  columns: TableColumn<T>[];
  totalCount: number;
  page: number;
  rowsPerPage: number;
  orderBy: string;
  order: 'asc' | 'desc';
  selected: string[];
  filterName: string;
  onFilterName: (event: React.ChangeEvent<HTMLInputElement>) => void;
  onChangePage: (event: unknown, newPage: number) => void;
  onChangeRowsPerPage: (event: React.ChangeEvent<HTMLInputElement>) => void;
  onSort: (property: keyof T) => void;
  onSelectAllRows: (checked: boolean) => void;
  onSelectRow: (event: React.ChangeEvent<HTMLInputElement>, checked: boolean, id: string) => void;
  getRowId: (row: T) => string;
  actions?: TableAction<T, C>[];
  actionContext?: C;  
  expandableContent?: (row: any) => React.ReactNode;
  customFilters?: ReactNode;
}

export interface TableAction<T, C = any> {
  label: string;
  icon?: string;
  onClick: (row: T, context?: C) => void;
  sx?: React.CSSProperties;
}

export function GenericTable<T, C = any>({
  data,
  columns,
  totalCount,
  page,
  rowsPerPage,
  orderBy,
  order,
  selected,
  filterName,
  onFilterName,
  onChangePage,
  onChangeRowsPerPage,
  onSort,
  onSelectAllRows,
  onSelectRow,
  getRowId,
  actions,
  expandableContent,
  customFilters
}: GenericTableProps<T, C>) {
  const [expandedRows, setExpandedRows] = useState<Set<string>>(new Set());

  const emptyRows = page > 0 ? Math.max(0, (1 + page) * rowsPerPage - totalCount) : 0;
  const isNotFound = !data.length && !!filterName;

  const handleExpandRow = (rowId: string) => {
    setExpandedRows(prev => {
      const newSet = new Set(prev);
      if (newSet.has(rowId)) {
        newSet.delete(rowId);
      } else {
        newSet.add(rowId);
      }
      return newSet;
    });
  };

  return (
    <Card>
      <GenericTableToolbar
        numSelected={selected.length}
        filterName={filterName}
        onFilterName={onFilterName}
        customFilters={customFilters}
      />
      <Scrollbar>
      <TableContainer sx={{ position: 'relative', overflow: 'unset' }}>
        <Table sx={{ minWidth: 800 }}>
          <GenericTableHead<T>
            order={order}
            orderBy={orderBy}
            rowCount={data.length}
            numSelected={selected.length}
            onSort={onSort}
            onSelectAllRows={onSelectAllRows}
            headLabel={columns}
            includeActions={actions ? Object.keys(actions).length > 0 : false}
            includeExpand={!!expandableContent}
          />
          <TableBody>
            {data.map((row) => (
              <React.Fragment key={getRowId(row)}>
                <GenericTableRow<T, C>
                  row={row}
                  columns={columns}
                  selected={selected.indexOf(getRowId(row)) !== -1}
                  onSelectRow={onSelectRow}
                  getRowId={getRowId}
                  actions={actions}
                  expandable={!!expandableContent}
                  expanded={expandedRows.has(getRowId(row))}
                  onExpandClick={() => handleExpandRow(getRowId(row))}
                />
                {expandableContent && (
                  <TableRow>
                    <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={columns.length + 2}>
                      <Collapse in={expandedRows.has(getRowId(row))} timeout="auto" unmountOnExit>
                        <Box sx={{ margin: 1 }}>
                          {expandableContent(row)}
                        </Box>
                      </Collapse>
                    </TableCell>
                  </TableRow>
                )}
              </React.Fragment>
            ))}
            <TableEmptyRows height={68} emptyRows={emptyRows} />
            {isNotFound && <TableNoData searchQuery={filterName} />}
          </TableBody>
        </Table>
      </TableContainer>
      </Scrollbar>
      <TablePagination
        page={page}
        component="div"
        count={totalCount}
        rowsPerPage={rowsPerPage}
        onPageChange={onChangePage}
        rowsPerPageOptions={[5, 10, 25]}
        onRowsPerPageChange={onChangeRowsPerPage}
      />
    </Card>
  );
}