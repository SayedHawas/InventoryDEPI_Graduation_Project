import { TableRow, TableCell } from "@mui/material";

interface TableEmptyRowsProps {
    emptyRows: number;
    height: number;
  }
  
  export function TableEmptyRows({ emptyRows, height }: TableEmptyRowsProps) {
    if (emptyRows === 0) return null;
  
    return (
      <TableRow sx={{ height: height * emptyRows }}>
        <TableCell colSpan={9} />
      </TableRow>
    );
  }