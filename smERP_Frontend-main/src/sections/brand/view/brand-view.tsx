import { Box, Typography, Button, Card, TableContainer, CircularProgress } from "@mui/material";
import { useCallback, useState } from "react";
import { Iconify } from "src/components/iconify";
import { Scrollbar } from "src/components/scrollbar";
import { useEntities } from "src/hooks/use-entities";
import { useTable } from "src/hooks/use-table";
import { GenericTable, TableAction } from "src/layouts/components/table/generic-table";
import { DashboardContent } from "src/layouts/dashboard";
import { TableColumn } from "src/services/types";
import { BrandForm } from "../brand-form";
import { CustomDialog } from "src/layouts/components/custom-dialog";

type BrandProps = {
    id: string,
    name: string,
    productCount: number
}

const transformBrand = (apiBrand: any): BrandProps => {
    return {
        id: apiBrand.brandId,
        name: apiBrand.name,
        productCount: apiBrand.productCount,
    };
};

const BRAND_TABLE_COLUMNS: TableColumn<BrandProps>[] = [
    { id: 'name', label: 'Name' },
    { id: 'productCount', label: 'Product Count', sortable: false },
];

export function BrandView() {
    const [filterName, setFilterName] = useState('');
    const [showForm, setShowForm] = useState(false);
    const [selectedBrand, setSelectedBrand] = useState<BrandProps | null>(null);

    const table = useTable();

    const { entities: brands, loading, error, totalCount, updateParams, refetch } = useEntities<BrandProps>(
        'brands',
        {
            PageNumber: table.page + 1,
            PageSize: table.rowsPerPage,
            SortBy: table.orderBy,
            SortDescending: table.order === 'desc',
            SearchTerm: filterName,
        },
        transformBrand
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

    const handleSort = (property: keyof BrandProps) => {
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

    const handleAddBrand = () => {
        setSelectedBrand(null);
        setShowForm(true);
    };

    const handleEditBrand = (brand: BrandProps) => {
        setSelectedBrand(brand);
        setShowForm(true);
    };

    const handleFormClose = useCallback(() => {
        setShowForm(false);
        refetch();
    }, [refetch]);

    const handleFormCancel = () => {
        setShowForm(false);
    }

    const tableActions: TableAction<BrandProps>[] = [
        {
          label: 'Edit',
          icon: 'solar:pen-bold',
          onClick: (row) => handleEditBrand(row),
        }
      ];

    return (
        <DashboardContent>
            <Box display="flex" alignItems="center" mb={5}>
                <Typography variant="h4" flexGrow={1}>
                    Brands
                </Typography>
                <Button variant="contained" color="inherit" onClick={handleAddBrand} startIcon={<Iconify icon="mingcute:add-line" />}>
                    New brand
                </Button>
            </Box>

            <Card>
                <Scrollbar>
                    <TableContainer sx={{ position: 'relative', overflow: 'unset' }}>
                        <GenericTable
                            data={brands}
                            columns={BRAND_TABLE_COLUMNS}
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
                            onSelectAllRows={(checked) => table.onSelectAllRows(checked, brands.map(brand => brand.id))}
                            onSelectRow={handleSelectRow}
                            getRowId={(row: BrandProps) => row.id}
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
            <CustomDialog open={showForm} handleCancel={handleFormCancel} title={selectedBrand?.id ? 'Edit brand' : 'Add new brand'} content={<BrandForm brandId={selectedBrand?.id} onSubmitSuccess={handleFormClose} />} />
        </DashboardContent>
    );
}