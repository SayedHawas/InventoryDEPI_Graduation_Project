import { Box, Typography, Button, Card, TableContainer, CircularProgress } from "@mui/material";
import { useCallback, useState } from "react";
import { Iconify } from "src/components/iconify";
import { Scrollbar } from "src/components/scrollbar";
import { useEntities } from "src/hooks/use-entities";
import { useTable } from "src/hooks/use-table";
import { GenericTable, TableAction } from "src/layouts/components/table/generic-table";
import { DashboardContent } from "src/layouts/dashboard";
import { TableColumn } from "src/services/types";
import { CategoryForm } from "../category-form";
import { CustomDialog } from "src/layouts/components/custom-dialog";

type CategoryProps = {
    id: string,
    name: string,
    parentCategory: string,
    subCategories: string
}

const transformCategory = (apiCategory: any): CategoryProps => {
    return {
        id: apiCategory.categoryID,
        name: apiCategory.englishName,
        parentCategory: apiCategory.parentCategory?.label ?? 'No Parent Category',
        subCategories: apiCategory.subCategories
            ? apiCategory.subCategories.map((category: { label: string }) => category.label).join(', ')
            : 'No Child Categories',
    };
};

const CATEGORY_TABLE_COLUMNS: TableColumn<CategoryProps>[] = [
    { id: 'name', label: 'Name' },
    { id: 'parentCategory', label: 'Parent Category' },
    { id: 'subCategories', label: 'Child Categories' },
];

export function CategoryView(){
    const [filterName, setFilterName] = useState('');
    const [showForm, setShowForm] = useState(false);
    const [selectedCategory, setSelectedCategory] = useState<CategoryProps | null>(null);

    const table = useTable();

    const { entities: categories, loading, error, totalCount, updateParams, refetch } = useEntities<CategoryProps>(
        'categories',
        {
            PageNumber: table.page + 1,
            PageSize: table.rowsPerPage,
            SortBy: table.orderBy,
            SortDescending: table.order === 'desc',
            SearchTerm: filterName,
        },
        transformCategory
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

    const handleSort = (property: keyof CategoryProps) => {
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

    const handleAddCategory = () => {
        setSelectedCategory(null);
        setShowForm(true);
    };

    const handleEditCategory = (category: CategoryProps) => {
        setSelectedCategory(category);
        setShowForm(true);
    };

    const handleFormClose = useCallback(() => {
        setShowForm(false);
        refetch();
    }, [refetch]);

    const handleFormCancel = () =>{
        setShowForm(false);
    }

    const tableActions: TableAction<CategoryProps>[] = [
        {
          label: 'Edit',
          icon: 'solar:pen-bold',
          onClick: (row) => handleEditCategory(row),
        }
      ];

    return (
        <DashboardContent>
            <Box display="flex" alignItems="center" mb={5}>
                <Typography variant="h4" flexGrow={1}>
                Categories
                </Typography>
                <Button variant="contained" color="inherit" onClick={handleAddCategory} startIcon={<Iconify icon="mingcute:add-line" />}>
                    New category
                </Button>
            </Box>

            <Card>
                <Scrollbar>
                    <TableContainer sx={{ position: 'relative', overflow: 'unset' }}>
                        <GenericTable
                            data={categories}
                            columns={CATEGORY_TABLE_COLUMNS}
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
                            onSelectAllRows={(checked) => table.onSelectAllRows(checked, categories.map(category => category.id))}
                            onSelectRow={handleSelectRow}
                            getRowId={(row: CategoryProps) => row.id}
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
            <CustomDialog open={showForm} handleCancel={handleFormCancel} title={selectedCategory?.id ? 'Edit branch' : 'Add new branch'} content={<CategoryForm categoryId={selectedCategory?.id} onSubmitSuccess={handleFormClose} />} />
        </DashboardContent>
    )
}