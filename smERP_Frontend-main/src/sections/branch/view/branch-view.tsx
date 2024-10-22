import { Box, Typography, Button, Card, TableContainer, CircularProgress, Table, TableBody, TableCell, TableHead, TableRow } from "@mui/material";
import { useCallback, useState } from "react";
import { Iconify } from "src/components/iconify";
import { Scrollbar } from "src/components/scrollbar";
import { useEntities } from "src/hooks/use-entities";
import { useTable } from "src/hooks/use-table";
import { GenericTable, TableAction } from "src/layouts/components/table/generic-table";
import { DashboardContent } from "src/layouts/dashboard";
import { TableColumn } from "src/services/types";
import { BranchForm } from "../branch-form";
import { CustomDialog } from "src/layouts/components/custom-dialog";
import { GenericTableRow } from "src/layouts/components/table/generic-table-row";
import { StorageLocationForm } from "../storage-location-form";

type BranchProps = {
    id: string,
    name: string,
    storageLocations2: StorageLocation[]
    storageLocations: string
}

type StorageLocation = {
    id: string,
    name: string
}

const transformBranch = (apiBranch: any): BranchProps => {
    const locations = apiBranch.storageLocations || [];
    return {
        id: apiBranch.branchId,
        name: apiBranch.name,
        storageLocations2: locations.map((location: any) => ({
            id: location.value,
            name: location.label
        })),
        storageLocations: locations.length > 0
            ? locations.map((location: any) => location.label).join(', ')
            : 'No Storage Locations',
    };
};

const BRANCH_TABLE_COLUMNS: TableColumn<BranchProps>[] = [
    { id: 'name', label: 'Name' },
    { id: 'storageLocations', label: 'Storage Locations', sortable: false },
];

export function BranchView() {
    const [filterName, setFilterName] = useState('');
    const [showBranchForm, setBranchShowForm] = useState(false);
    const [showStorageLocationForm, setShowStorageLocationForm] = useState(false);
    const [selectedBranch, setSelectedBranch] = useState<BranchProps | null>(null);
    const [selectedStorageLocation, setSelectedStorageLocation] = useState<StorageLocation | null>(null);

    const table = useTable();

    const { entities: branches, loading, error, totalCount, updateParams, refetch } = useEntities<BranchProps>(
        'branches',
        {
            PageNumber: table.page + 1,
            PageSize: table.rowsPerPage,
            SortBy: table.orderBy,
            SortDescending: table.order === 'desc',
            SearchTerm: filterName,
        },
        transformBranch
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

    const handleSort = (property: keyof BranchProps) => {
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

    const handleAddBranch = () => {
        setSelectedBranch(null);
        setBranchShowForm(true);
    };

    const handleEditBranch = (branch: BranchProps) => {
        setSelectedBranch(branch);
        setBranchShowForm(true);
    };

    const handleAddStorageLocation = (branch: BranchProps) => {
        setSelectedBranch(branch)
        setSelectedStorageLocation(null)
        setShowStorageLocationForm(true)
    }

    const handleEditStorageLocation = (branch: BranchProps, storageLocation: StorageLocation) => {
        setSelectedBranch(branch)
        setSelectedStorageLocation(storageLocation)
        setShowStorageLocationForm(true)
    }

    const handleStorageLocationClose = useCallback(() => {
        setShowStorageLocationForm(false);
        refetch();
    }, [refetch]);

    const handleStorageLocationCancel = () => {
        setShowStorageLocationForm(false);
    }

    const handleBranchFormClose = useCallback(() => {
        setBranchShowForm(false);
        refetch();
    }, [refetch]);

    const handleBranchFormCancel = () => {
        setBranchShowForm(false);
    }

    const tableActions: TableAction<BranchProps>[] = [
        {
            label: 'Add Storage Location',
            icon: 'mingcute:add-fill',
            onClick: (row) => handleAddStorageLocation(row),
        },
        {
            label: 'Edit',
            icon: 'solar:pen-bold',
            onClick: (row) => handleEditBranch(row),
        }
    ];

    const childTableActions: TableAction<StorageLocation>[] = [
        {
            label: 'Edit',
            icon: 'solar:pen-bold',
            onClick: (row, branch) => handleEditStorageLocation(branch, row),
        }
    ]

    return (
        <DashboardContent>
            <Box display="flex" alignItems="center" mb={5}>
                <Typography variant="h4" flexGrow={1}>
                    Branches
                </Typography>
                <Button variant="contained" color="inherit" onClick={handleAddBranch} startIcon={<Iconify icon="mingcute:add-line" />}>
                    New branch
                </Button>
            </Box>

            <Card>
                <Scrollbar>
                    <TableContainer sx={{ position: 'relative', overflow: 'unset' }}>
                        <GenericTable
                            data={branches}
                            columns={BRANCH_TABLE_COLUMNS}
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
                            onSelectAllRows={(checked) => table.onSelectAllRows(checked, branches.map(branch => branch.id))}
                            onSelectRow={handleSelectRow}
                            getRowId={(row: BranchProps) => row.id}
                            actions={tableActions}
                            expandableContent={(branch: BranchProps) => (
                                <>
                                    <Typography variant="h6" gutterBottom component="div">
                                        Storage Locations
                                    </Typography>
                                    <Table size="small" aria-label="instances">
                                        <TableHead>
                                            <TableRow>
                                                <TableCell>Name</TableCell>
                                                <TableCell />
                                            </TableRow>
                                        </TableHead>
                                        <TableBody>
                                            {branch.storageLocations2?.map((storageLocation) => (
                                                <GenericTableRow<StorageLocation, BranchProps>
                                                    key={storageLocation.id}
                                                    row={storageLocation}
                                                    columns={[
                                                        { id: 'name', label: 'Name', align: 'left' },
                                                    ]}
                                                    selected={false}
                                                    getRowId={(row) => row.id}
                                                    actions={childTableActions}
                                                    actionContext={branch}
                                                    expandable={false}
                                                />
                                            ))}
                                        </TableBody>
                                    </Table>
                                </>
                            )}
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
            <CustomDialog open={showBranchForm} handleCancel={handleBranchFormCancel} title={selectedBranch?.id ? 'Edit branch' : 'Add new branch'} content={<BranchForm branchId={selectedBranch?.id} onSubmitSuccess={handleBranchFormClose} />} />
            <CustomDialog open={showStorageLocationForm} handleCancel={handleStorageLocationCancel} title={selectedBranch?.id ? 'Edit storage location' : 'Add new storage location'} content={<StorageLocationForm storageLocationsId={selectedStorageLocation?.id} branchId={selectedBranch?.id ?? ''} onSubmitSuccess={handleStorageLocationClose} />} />
        </DashboardContent>
    )
}