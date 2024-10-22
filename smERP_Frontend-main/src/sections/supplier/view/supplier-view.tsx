import { Box, Typography, Button, Card, TableContainer, CircularProgress } from "@mui/material";
import { useState, useCallback } from "react";
import { Iconify } from "src/components/iconify";
import { Scrollbar } from "src/components/scrollbar";
import { useEntities } from "src/hooks/use-entities";
import { useTable } from "src/hooks/use-table";
import { GenericTable, TableAction } from "src/layouts/components/table/generic-table";
import { DashboardContent } from "src/layouts/dashboard";
import { TableColumn } from "src/services/types";
import { SupplierForm } from "../supplier-form";
import { CustomDialog } from "src/layouts/components/custom-dialog";

type SupplierProps = {
    id: string,
    name: string,
    addresses: string,
    suppliedProducts: string
}

const transformSupplier = (apiSupplier: any): SupplierProps => {
    const formattedAddresses = apiSupplier.addresses.map((address: any, index: number) => (
        `Address ${index + 1}: ${address.street}, ${address.city}, ${address.state}, ${address.country}, ${address.postalCode} (Comment: ${address.comment})`
    )).join('\n');

    const formattedProducts = apiSupplier.products.map((product: any, index: number) => (
        `Product ${index + 1}: ${product.name} (ID: ${product.productId}, First Supplied: ${new Date(product.firstTime).toLocaleDateString()}, Last Supplied: ${new Date(product.lastTime).toLocaleDateString()})`
    )).join('\n');

    return {
        id: apiSupplier.supplierId,
        name: apiSupplier.name,
        addresses: formattedAddresses || 'No Address Known',
        suppliedProducts: formattedProducts || 'No Products Supplied Yet'
    };
};

const SUPPLIER_TABLE_COLUMNS: TableColumn<SupplierProps>[] = [
    { id: 'name', label: 'Name' },
    {
        id: 'addresses',
        label: 'Addresses',
        sortable: false,
        render: (row: SupplierProps) => (
            <Typography component="div" variant="body2" whiteSpace="pre-line">
                {row.addresses}
            </Typography>
        ),
    },
    { id: 'suppliedProducts', label: 'Products', sortable: false },
];

export function SupplierView(){
    const [filterName, setFilterName] = useState('');
    const [showForm, setShowForm] = useState(false);
    const [selectedSupplier, setSelectedSupplier] = useState<SupplierProps | null>(null);

    const table = useTable();

    const { entities: suppliers, loading, error, totalCount, updateParams, refetch } = useEntities<SupplierProps>(
        'suppliers',
        {
            PageNumber: table.page + 1,
            PageSize: table.rowsPerPage,
            SortBy: table.orderBy,
            SortDescending: table.order === 'desc',
            SearchTerm: filterName,
        },
        transformSupplier
    );

    const handleFilterName = (event: React.ChangeEvent<HTMLInputElement>) => {
        const newFilterName = event.target.value;
        setFilterName(newFilterName);
        updateParams({ SearchTerm: newFilterName, PageNumber: 1 });
        table.onChangePage(null, 0);
    };

    const handleChangePage = (event: unknown, newPage: number) => {
        updateParams({ PageNumber: newPage + 1 });
        table.onChangePage(event, newPage);
    };

    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        const newRowsPerPage = parseInt(event.target.value, 10);
        updateParams({ PageSize: newRowsPerPage, PageNumber: 1 });
        table.onChangeRowsPerPage(event);
    };

    const handleSort = (property: keyof SupplierProps) => {
        const isAsc = table.orderBy === property && table.order === 'asc';
        updateParams({
            SortBy: property,
            SortDescending: !isAsc,
        });
        table.onSort(property);
    };

    const handleSelectRow = (event: React.ChangeEvent<HTMLInputElement>, checked: boolean, id: string) => {
        table.onSelectRow(event, checked, id);
    };

    const handleAddSupplier = () => {
        setSelectedSupplier(null);
        setShowForm(true);
    };

    const handleEditSupplier = (brand: SupplierProps) => {
        setSelectedSupplier(brand);
        setShowForm(true);
    };

    const handleFormClose = useCallback(() => {
        setShowForm(false);
        refetch();
    }, [refetch]);

    const handleFormCancel = () => {
        setShowForm(false);
    }

    const tableActions: TableAction<SupplierProps>[] = [
        {
          label: 'Edit',
          icon: 'solar:pen-bold',
          onClick: (row) => handleEditSupplier(row),
        }
      ];

    return (
        <DashboardContent>
            <Box display="flex" alignItems="center" mb={5}>
                <Typography variant="h4" flexGrow={1}>
                    Suppliers
                </Typography>
                <Button variant="contained" color="inherit" onClick={handleAddSupplier} startIcon={<Iconify icon="mingcute:add-line" />}>
                    New supplier
                </Button>
            </Box>

            <Card>
                <Scrollbar>
                    <TableContainer sx={{ position: 'relative', overflow: 'unset' }}>
                        <GenericTable
                            data={suppliers}
                            columns={SUPPLIER_TABLE_COLUMNS}
                            totalCount={totalCount}
                            page={table.page}
                            rowsPerPage={table.rowsPerPage}
                            orderBy={table.orderBy}
                            order={table.order}
                            selected={table.selected}
                            filterName={filterName}
                            onFilterName={handleFilterName}
                            onChangePage={handleChangePage}
                            onChangeRowsPerPage={handleChangeRowsPerPage}
                            onSort={handleSort}
                            onSelectAllRows={(checked) => table.onSelectAllRows(checked, suppliers.map(supplier => supplier.id))}
                            onSelectRow={handleSelectRow}
                            getRowId={(row: SupplierProps) => row.id}
                            actions={tableActions}
                        />
                        {loading && (
                            <Box
                                sx={{
                                    position: 'absolute',
                                    top: 0,
                                    left: 0,
                                    right: 0,
                                    bottom: 0,
                                    bgcolor: 'rgba(255, 255, 255, 0.8)',
                                    display: 'flex',
                                    justifyContent: 'center',
                                    alignItems: 'center',
                                    zIndex: 10,
                                }}
                            >
                                <CircularProgress />
                            </Box>
                        )}

                        {error && (
                            <Box
                                sx={{
                                    position: 'absolute',
                                    top: 0,
                                    left: 0,
                                    right: 0,
                                    bottom: 0,
                                    bgcolor: 'rgba(255, 255, 255, 0.8)',
                                    display: 'flex',
                                    justifyContent: 'center',
                                    alignItems: 'center',
                                    zIndex: 10,
                                }}
                            >
                                {error}
                            </Box>
                        )}
                    </TableContainer>
                </Scrollbar>
            </Card>
            <CustomDialog open={showForm} handleCancel={handleFormCancel} title={selectedSupplier?.id ? 'Edit supplier' : 'Add new supplier'} content={<SupplierForm supplierId={selectedSupplier?.id} onSubmitSuccess={handleFormClose} />} />
        </DashboardContent>
    );

}