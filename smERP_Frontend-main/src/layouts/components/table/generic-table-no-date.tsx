import { TableRow, TableCell, Paper, Typography } from "@mui/material";

interface TableNoDataProps {
    searchQuery?: string;
  }
  
  export function TableNoData({ searchQuery }: TableNoDataProps) {
    return (
      <TableRow>
        <TableCell align="center" colSpan={9} sx={{ py: 3 }}>
          <Paper sx={{ textAlign: 'center' }}>
            <Typography variant="h6" paragraph>
              Not found
            </Typography>
            <Typography variant="body2">
              No results found for &nbsp;
              <strong>&quot;{searchQuery}&quot;</strong>.
              <br /> Try checking for typos or using complete words.
            </Typography>
          </Paper>
        </TableCell>
      </TableRow>
    );
  }