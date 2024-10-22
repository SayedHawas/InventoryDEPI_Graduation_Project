import { TableHead, TableRow, TableCell, Checkbox, TableSortLabel, Box } from "@mui/material";
import { TableColumn } from "src/services/types";

interface GenericTableHeadProps<T> {
  order: 'asc' | 'desc';
  orderBy: string;
  rowCount: number;
  numSelected: number;
  onSort: (property: keyof T) => void;
  onSelectAllRows: (checked: boolean) => void;
  headLabel: TableColumn<T>[];
  includeActions?: boolean;
  includeExpand?: boolean;
}

export function GenericTableHead<T>({
  order,
  orderBy,
  rowCount,
  numSelected,
  onSort,
  onSelectAllRows,
  headLabel,
  includeActions = false,
  includeExpand = false,
}: GenericTableHeadProps<T>) {
  return (
    <TableHead>
      <TableRow>
        {includeExpand && <TableCell />}
        <TableCell padding="checkbox">
          <Checkbox
            indeterminate={numSelected > 0 && numSelected < rowCount}
            checked={rowCount > 0 && numSelected === rowCount}
            onChange={(event) => onSelectAllRows(event.target.checked)}
          />
        </TableCell>
        {headLabel.map((headCell) => (
          <TableCell
            key={headCell.id as string}
            align={headCell.align || 'left'}
            sortDirection={orderBy === headCell.id ? order : false}
          >
            {(headCell.sortable === undefined ? true : headCell.sortable) ? (
              <TableSortLabel
                hideSortIcon
                active={orderBy === headCell.id}
                direction={orderBy === headCell.id ? order : 'asc'}
                onClick={() => onSort(headCell.id as keyof T)}
              >
                {headCell.label}
                {orderBy === headCell.id ? (
                  <Box sx={{ ...visuallyHidden }}>
                    {order === 'desc' ? 'sorted descending' : 'sorted ascending'}
                  </Box>
                ) : null}
              </TableSortLabel>
            ) : (
              headCell.label
            )}
          </TableCell>
        ))}
        {includeActions && (
          <TableCell />
        )}
      </TableRow>
    </TableHead>
  );
}
  
  const visuallyHidden = {
    border: 0,
    margin: -1,
    padding: 0,
    width: '1px',
    height: '1px',
    overflow: 'hidden',
    position: 'absolute',
    whiteSpace: 'nowrap',
    clip: 'rect(0 0 0 0)',
  };