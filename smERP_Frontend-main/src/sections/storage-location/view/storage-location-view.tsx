import { Box, Typography, Button, Card, TableContainer, Table, TableHead, TableRow, TableCell, TableBody, CircularProgress, SelectChangeEvent } from "@mui/material"
import { useCallback, useState } from "react"
import { FormSelectField } from "src/components/form-fields/form-select-field"
import { Iconify } from "src/components/iconify"
import { useAuth } from "src/contexts/AuthContext"
import { useBranches } from "src/hooks/use-branches"
import { useEntities } from "src/hooks/use-entities"
import { useTable } from "src/hooks/use-table"
import { CustomDialog } from "src/layouts/components/custom-dialog"
import { GenericTable, TableAction } from "src/layouts/components/table/generic-table"
import { GenericTableRow } from "src/layouts/components/table/generic-table-row"
import { DashboardContent } from "src/layouts/dashboard"
import { AttributeForm } from "src/sections/attribute/attribute-form"
import { StorageLocationForm } from "src/sections/branch/storage-location-form"
import { ProductForm } from "src/sections/product/product-form"
import { ProductInstanceForm } from "src/sections/product/product-instance-form"
import { TableColumn } from "src/services/types"

type StorageLocationProps = {
    id: string,
    name: string
    products?: ProductProps[]
}

type ProductProps = {
    id: string,
    name: string,
    sku: string,
    quantity: number,
    buyPrice: number,
    sellPrice: number
}

const transformStorageLocation = (apiResponse: any): StorageLocationProps => {
    return {
        id: apiResponse.storageLocationId,
        name: apiResponse.name,
        products: Array.isArray(apiResponse.products) ? apiResponse.products.map((product: any) => ({
            id: product.productInstanceId,
            sku: product.sku,
            name: product.name,
            quantity: product.quantity,
            buyPrice: product.buyingPrice,
            sellPrice: product.sellingPrice
        })) : [],
    };
};


const STORAGE_LOCATION_TABLE_COLUMNS: TableColumn<StorageLocationProps>[] = [
    { id: 'name', label: 'Name' },
    {
        id: 'productCount',
        label: 'Number of Products',
        render: (location: StorageLocationProps) => (
            <span>{location.products?.length || 0}</span>
        ),
    }
];

const PRODUCT_TABLE_COLUMNS: TableColumn<ProductProps>[] = [
    { id: 'name', label: 'Name' },
    { id: 'sku', label: 'SKU' },
    { id: 'quantity', label: 'Quantity' },
    { id: 'buyPrice', label: 'Buying Price' },
    { id: 'sellPrice', label: 'Selling Price' },
];


export function StorageLocationView() {

    const { user } = useAuth();

    const isAdmin = Array.isArray(user?.roles)
    ? user.roles.some(role => role === 'Admin')
    : user?.roles === 'Admin';

    const { data: branches } = useBranches()

    const [filterName, setFilterName] = useState('');
    const [showForm, setShowForm] = useState(false);
    const [branchId, setBranchId] = useState(user?.branch ?? '');
    const [selectedStorageLocation, setSelectedStorageLocation] = useState<StorageLocationProps | null>(null);
    const [selectedProduct, setSelectedProduct] = useState<ProductProps | null>(null);

    const table = useTable();

    console.log(user, branchId)

    const { entities: storageLocations, loading, error, totalCount, updateParams, refetch } = useEntities<StorageLocationProps>(
        `branches/${branchId}/storage-locations`,
        {
            PageNumber: table.page + 1,
            PageSize: table.rowsPerPage,
            SortBy: table.orderBy,
            SortDescending: table.order === 'desc',
            SearchTerm: filterName,
        },
        transformStorageLocation
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

    const handleSort = (property: keyof StorageLocationProps) => {
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

    const handleAddStorageLocation = () => {
        setSelectedStorageLocation(null);
        setShowForm(true);
    };

    const handleEditStorageLocation = (storageLocation: StorageLocationProps) => {
        setSelectedStorageLocation(storageLocation);
        setShowForm(true);
    };

    const handleFormClose = useCallback(() => {
        setShowForm(false);
        refetch();
    }, [refetch]);

    const handleFormCancel = () => {
        setShowForm(false);
    }

    const handleBranchChange = (value: string, event: SelectChangeEvent<unknown>) => {
        setBranchId(value);
        refetch();
    }

    const tableActions: TableAction<StorageLocationProps>[] = [
        {
            label: 'Edit',
            icon: 'solar:pen-bold',
            onClick: (row) => handleEditStorageLocation(row),
        }
    ];

    return (
        <DashboardContent>
            <Box display="flex" alignItems="center" mb={5} flexDirection={{ xs: 'column', sm: 'row' }} gap={1}>
                <Typography variant="h4" flexGrow={1}>
                    Storage Locations
                </Typography>
                <Button variant="contained" color="inherit" onClick={handleAddStorageLocation} startIcon={<Iconify icon="mingcute:add-line" />}>
                    New storage location
                </Button>
            </Box>

            <Card>
                <TableContainer sx={{ position: 'relative', overflow: 'unset' }}>
                    <GenericTable
                        data={storageLocations}
                        columns={STORAGE_LOCATION_TABLE_COLUMNS}
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
                        onSelectAllRows={(checked) => table.onSelectAllRows(checked, storageLocations.map(storageLocations => storageLocations.id))}
                        onSelectRow={handleSelectRow}
                        getRowId={(row: StorageLocationProps) => row.id}
                        actions={tableActions}
                        customFilters={
                            isAdmin && (<>
                                <FormSelectField
                                    label="Branch"
                                    options={branches?.value ?? []}
                                    onChange={handleBranchChange}
                                    defaultValue=""
                                />
                            </>)
                        }
                        expandableContent={(storageLocation: StorageLocationProps) => (
                            <>
                                <Typography variant="h6" gutterBottom component="div">
                                    Products
                                </Typography>
                                <Table size="small" aria-label="instances">
                                    <TableHead>
                                        <TableRow>
                                            <TableCell>Name</TableCell>
                                            <TableCell>SKU</TableCell>
                                            <TableCell>Quantity</TableCell>
                                            <TableCell>Buy Price</TableCell>
                                            <TableCell>Sell Price</TableCell>
                                            <TableCell />
                                        </TableRow>
                                    </TableHead>
                                    <TableBody>
                                        {storageLocation.products?.map((product) => (
                                            <GenericTableRow<ProductProps>
                                                key={product.id}
                                                row={product}
                                                columns={PRODUCT_TABLE_COLUMNS}
                                                selected={false}
                                                getRowId={(row) => row.id}
                                                expandable={false}
                                            />
                                        ))}
                                    </TableBody>
                                </Table>
                            </>
                        )} />
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
            </Card>
            <CustomDialog open={showForm} handleCancel={handleFormCancel} title={'Add new storage location'} content={<StorageLocationForm onSubmitSuccess={handleFormClose} branchId={branchId} />} />
        </DashboardContent>
    )
}