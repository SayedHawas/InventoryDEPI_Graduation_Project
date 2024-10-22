import { Box, Typography, Button, Card, TableContainer, CircularProgress } from "@mui/material";
import { useState, useCallback } from "react";
import { Iconify } from "src/components/iconify";
import { Scrollbar } from "src/components/scrollbar";
import { useEntities } from "src/hooks/use-entities";
import { useTable } from "src/hooks/use-table";
import { CustomDialog } from "src/layouts/components/custom-dialog";
import { GenericTable, TableAction } from "src/layouts/components/table/generic-table";
import { DashboardContent } from "src/layouts/dashboard";
import { TableColumn } from "src/services/types";
import { AttributeForm } from "../attribute-form";

type AttributeProps = {
    id: string,
    name: string,
    values: string
}

const transformAttribute = (apiAttribute: any): AttributeProps => {
    return {
        id: apiAttribute.value,
        name: apiAttribute.label,
        values: apiAttribute.values
            ? apiAttribute.values.map((value: { label: string }) => value.label).join(', ')
            : 'No Values',
    };
};

const ATTRIBUTE_TABLE_COLUMNS: TableColumn<AttributeProps>[] = [
    { id: 'name', label: 'Name' },
    { id: 'values', label: 'Values', sortable: false },
];

export function AttributeView() {
    const [filterName, setFilterName] = useState('');
    const [showForm, setShowForm] = useState(false);
    const [selectedAttribute, setSelectedAttribute] = useState<AttributeProps | null>(null);

    const table = useTable();

    const { entities: attributes, loading, error, totalCount, updateParams, refetch } = useEntities<AttributeProps>(
        'attributes',
        {
            PageNumber: table.page + 1,
            PageSize: table.rowsPerPage,
            SortBy: table.orderBy,
            SortDescending: table.order === 'desc',
            SearchTerm: filterName,
        },
        transformAttribute
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

    const handleSort = (property: keyof AttributeProps) => {
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

    const handleAddAttribute = () => {
        setSelectedAttribute(null);
        setShowForm(true);
    };

    const handleEditAttribute = (brand: AttributeProps) => {
        setSelectedAttribute(brand);
        setShowForm(true);
    };

    const handleFormClose = useCallback(() => {
        setShowForm(false);
        refetch();
    }, [refetch]);

    const handleFormCancel = () => {
        setShowForm(false);
    }

    const tableActions: TableAction<AttributeProps>[] = [
        {
          label: 'Edit',
          icon: 'solar:pen-bold',
          onClick: (row) => handleEditAttribute(row),
        }
      ];

    return (
        <DashboardContent>
            <Box display="flex" alignItems="center" mb={5}>
                <Typography variant="h4" flexGrow={1}>
                    Attributes
                </Typography>
                <Button variant="contained" color="inherit" onClick={handleAddAttribute} startIcon={<Iconify icon="mingcute:add-line" />}>
                    New attribute
                </Button>
            </Box>

            <Card>
                <Scrollbar>
                    <TableContainer sx={{ position: 'relative', overflow: 'unset' }}>
                        <GenericTable
                            data={attributes}
                            columns={ATTRIBUTE_TABLE_COLUMNS}
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
                            onSelectAllRows={(checked) => table.onSelectAllRows(checked, attributes.map(attribute => attribute.id))}
                            onSelectRow={handleSelectRow}
                            getRowId={(row: AttributeProps) => row.id}
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
            <CustomDialog open={showForm} handleCancel={handleFormCancel} title={selectedAttribute?.id ? 'Edit attribute' : 'Add new attribute'} content={<AttributeForm attributeId={selectedAttribute?.id} onSubmitSuccess={handleFormClose} />} />
        </DashboardContent>
    )
}