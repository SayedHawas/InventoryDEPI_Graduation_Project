import React, { useCallback, useMemo, useState } from 'react';
import { Box, Button, Card, CircularProgress, TableContainer, Typography } from '@mui/material';
import { useEntities } from 'src/hooks/use-entities';
import { useTable } from 'src/hooks/use-table';
import { GenericTable, TableAction } from 'src/layouts/components/table/generic-table';
import { SelectOption, TableColumn } from 'src/services/types';
import { Label } from 'src/components/label';
import { Iconify } from 'src/components/iconify';
import { Scrollbar } from 'src/components/scrollbar';
import { DashboardContent } from 'src/layouts/dashboard';
import { CustomDialog } from 'src/layouts/components/custom-dialog';
import { AddEditUserForm } from '../add-new-user';
import { useBranches } from 'src/hooks/use-branches';

type UserProps = {
  id: string;
  UserName: string;
  Email: string;
  role: string;
  status: string;
  branchId: number;
  branch: string;
  PhoneNumber: string | null;
};

const USER_TABLE_COLUMNS: TableColumn<UserProps>[] = [
  { id: 'UserName', label: 'Name' },
  { id: 'Email', label: 'Email' },
  { id: 'role', label: 'Role', sortable: false },
  { id: 'branch', label: 'Branch', sortable: false },
  { id: 'PhoneNumber', label: 'Phone Number' },
  {
    id: 'status',
    label: 'Status',
    align: 'center',
    render: (user) => (
      <Label color={(user.status === 'banned' && 'error') || 'success'}>{user.status}</Label>
    ),
  },
];

const transformUser = (apiUser: any): Omit<UserProps, 'branch'> => ({
  id: apiUser.userId,
  UserName: apiUser.userName,
  Email: apiUser.email,
  role: apiUser.roles?.join(', ') || 'No Role',
  branchId: apiUser.branchId,
  status: apiUser.isAccountDisabled ? 'banned' : 'active',
  PhoneNumber: apiUser.phoneNumber ?? 'Not Available',
});

export function UserView() {
  const [filterName, setFilterName] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [selectedUser, setSelectedUser] = useState<UserProps | null>(null);

  const table = useTable();

  const { data: branchesData } = useBranches();
  const branches = useMemo(() =>
    branchesData?.value.reduce((acc: Record<string, string>, branch: SelectOption) => {
      acc[String(branch.value)] = branch.label;
      return acc;
    }, {}) || {},
    [branchesData]
  );

  const { entities: usersWithoutBranch, loading, error, totalCount, updateParams, refetch } = useEntities<Omit<UserProps, 'branch'>>(
    'auth/users',
    {
      PageNumber: table.page + 1,
      PageSize: table.rowsPerPage,
      SortBy: table.orderBy,
      SortDescending: table.order === 'desc',
      SearchTerm: filterName,
    },
    transformUser
  );

  const users = useMemo(() =>
    usersWithoutBranch.map(user => ({
      ...user,
      branch: branches[user.branchId] || 'Unknown'
    })),
    [usersWithoutBranch, branches]
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

  const handleSort = (property: keyof UserProps) => {
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

  const handleAddUser = () => {
    setSelectedUser(null);
    setShowForm(true);
  };

  const handleFormClose = useCallback(() => {
    setShowForm(false);
    refetch();
  }, [refetch]);

  const handleFormCancel = () => {
    setShowForm(false);
  }


  const handleEditUser = (user: UserProps) => {
    setSelectedUser(user);
    setShowForm(true);
  };

  const tableActions: TableAction<UserProps>[] = [
    {
      label: 'Edit',
      icon: 'solar:pen-bold',
      onClick: (row) => handleEditUser(row),
    }
  ];

  return (
    <DashboardContent>
      <Box display="flex" alignItems="center" mb={5}>
        <Typography variant="h4" flexGrow={1}>
          Users
        </Typography>
        <Button variant="contained" color="inherit" onClick={handleAddUser} startIcon={<Iconify icon="mingcute:add-line" />}>
          New user
        </Button>
      </Box>

      <Card>
        <Scrollbar>
          <TableContainer sx={{ position: 'relative', overflow: 'unset' }}>
            <GenericTable
              data={users}
              columns={USER_TABLE_COLUMNS}
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
              onSelectAllRows={(checked) => table.onSelectAllRows(checked, users.map(user => user.id))}
              onSelectRow={handleSelectRow}
              getRowId={(row: UserProps) => row.id}
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
      <CustomDialog open={showForm} handleCancel={handleFormCancel} title={selectedUser?.id ? 'Edit user' : 'Add new user'} content={<AddEditUserForm userId={selectedUser?.id} onSubmitSuccess={handleFormClose} />} />
    </DashboardContent>
  );
}