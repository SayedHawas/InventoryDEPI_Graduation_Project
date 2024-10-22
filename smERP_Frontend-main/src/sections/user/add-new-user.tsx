import { useForm, SubmitHandler } from 'react-hook-form';
import { Box, CircularProgress, Typography } from '@mui/material';
import { LoadingButton } from '@mui/lab';
import { FormField } from 'src/components/form-fields/form-field';
import { AddressFieldGroup } from 'src/components/form-fields/address-form-group';
import { FormSelectField } from 'src/components/form-fields/form-select-field';
import { AutocompleteMultiSelectField } from 'src/components/form-fields/autocomplete-multi-select-field';
import { useEffect, useState } from 'react';
import { FormPhoneNumberField } from 'src/components/form-fields/tel-form-field';
import { useBranches } from 'src/hooks/use-branches';
import { useRoles } from 'src/hooks/use-roles';
import { apiService } from 'src/services/api';
import { useAuth } from 'src/contexts/AuthContext';

export interface UserFormData {
  userId?: string;
  firstName: string;
  lastName: string;
  email: string;
  password?: string;
  confirmPassword?: string;
  address: {
    street: string;
    city: string;
    state: string;
    country: string;
    postalCode: string;
    comment?: string;
  };
  phoneNumber: string;
  branchId: string;
  roles: string[];
}

interface AddEditUserFormProps {
  userId?: string;
  onSubmitSuccess: () => void;
}

export function AddEditUserForm({ userId, onSubmitSuccess }: AddEditUserFormProps) {
  const [loading, setLoading] = useState(false);
  const [submissionError, setSubmissionError] = useState<string | null>(null);
  const [fetchingUser, setFetchingUser] = useState(false);
  const isEditMode = !!userId;

  const { user } = useAuth();

  const isAdmin = Array.isArray(user?.roles)
  ? user.roles.some(role => role === 'Admin')
  : user?.roles === 'Admin';

  const { control, handleSubmit, watch, reset, formState: { errors, dirtyFields } } = useForm<UserFormData>({
    defaultValues: {
      userId: '',
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      confirmPassword: '',
      phoneNumber: '',
      branchId: user?.branch,
      roles: [],
      address: {
        street: '',
        city: '',
        state: '',
        country: '',
        postalCode: '',
        comment: '',
      },
    },
  });

  const password = watch('password');

  useEffect(() => {
    const fetchUserData = async () => {
      if (userId) {
        setFetchingUser(true);
        try {
          const response = await fetch(`https://smerp.runasp.net/auth/users/${userId}`);
          if (!response.ok) {
            throw new Error('Failed to fetch user data');
          }
          const responseBody = await response.json();
          const userData: UserFormData = responseBody.value
          reset(userData);
        } catch (error) {
          console.error('Error fetching user data:', error);
          setSubmissionError('Failed to load user data. Please try again.');
        } finally {
          setFetchingUser(false);
        }
      }
    };

    fetchUserData();
  }, [userId, reset]);

  const onSubmit: SubmitHandler<UserFormData> = async (data) => {
    setLoading(true);
    setSubmissionError(null);
    try {
      if (isEditMode) {
        const changedData = Object.fromEntries(
          Object.entries(dirtyFields)
            .filter(([_, isDirty]) => isDirty)
            .map(([key]) => [key, data[key as keyof UserFormData]])
        ) as Partial<UserFormData>;
        await apiService.users.update(userId, changedData);
      } else {
        await apiService.users.create(data);
      }
      console.log(isEditMode ? 'User updated successfully' : 'User added successfully');
      onSubmitSuccess();
    } catch (error: any) {
      console.error(error);
      setSubmissionError(error.message || "An unexpected error occurred. Please try again.");
    } finally {
      setLoading(false);
    }
  };
  
  const { data: branchesResponse, error: branchesError, isLoading: isLoadingBranches } = useBranches();
  const { data: rolesResponse, error: rolesError, isLoading: isLoadingRoles } = useRoles();
  
  if (isLoadingBranches || isLoadingRoles || fetchingUser) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="100vh">
        <CircularProgress />
        <Typography variant="h6" sx={{ marginLeft: 2 }}>
          Loading...
        </Typography>
      </Box>
    );
  }

  if (branchesError || rolesError) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="100vh">
        <Typography color="error">{branchesError?.message} {rolesError?.message}</Typography>
      </Box>
    );
  }

  return (
    <Box component="form" autoComplete="on" onSubmit={handleSubmit(onSubmit)}>
      <FormField<UserFormData>
        name="firstName"
        control={control}
        label="First Name"
        rules={{ required: 'First name is required' }}
        error={!!errors.firstName}
        helperText={errors.firstName?.message}
      />

      <FormField<UserFormData>
        name="lastName"
        control={control}
        label="Last Name"
        rules={{ required: 'Last name is required' }}
        error={!!errors.lastName}
        helperText={errors.lastName?.message}
      />

      <FormField<UserFormData>
        name="email"
        type='email'
        control={control}
        label="Email"
        rules={{ required: 'Email is required' }}
        error={!!errors.email}
        helperText={errors.email?.message}
      />

      {!isEditMode && (
        <>
          <FormField<UserFormData>
            name="password"
            control={control}
            label="Password"
            type="password"
            showPasswordToggle
            rules={{
              required: 'Password is required',
              minLength: {
                value: 8,
                message: 'Password must be at least 8 characters long',
              },
            }}
            error={!!errors.password}
            helperText={errors.password?.message}
          />

          <FormField<UserFormData>
            name="confirmPassword"
            control={control}
            label="Confirm Password"
            type="password"
            showPasswordToggle
            rules={{
              required: 'Please confirm your password',
              validate: (value) => value === password || 'The passwords do not match',
            }}
            error={!!errors.confirmPassword}
            helperText={errors.confirmPassword?.message}
          />
        </>
      )}

      <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
        Address
      </Typography>
      <AddressFieldGroup<UserFormData> control={control} prefix="address" errors={errors.address} />

      <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
        Phone Number
      </Typography>
      <FormPhoneNumberField<UserFormData>
        name="phoneNumber"
        control={control}
        label="Phone Number"
        rules={{ required: 'Phone number is required' }}
        error={!!errors.phoneNumber}
        helperText={errors.phoneNumber?.message}
      />

      <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
        Branch
      </Typography>
      <FormSelectField<UserFormData>
        name="branchId"
        control={control}
        disabled={!isAdmin}
        rules={{ required: 'User Branch is required' }}
        error={!!errors.branchId}
        helperText={errors.branchId?.message}
        options={branchesResponse?.value || []}
      />

      <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
        User roles
      </Typography>
      <AutocompleteMultiSelectField<UserFormData>
        name="roles"
        control={control}
        rules={{ required: 'User Role is required' }}
        error={!!errors.roles}
        helperText={errors.roles?.message}
        options={rolesResponse?.value.map(role => ({ value: role, label: role })) || []}
      />

      <LoadingButton
        fullWidth
        size="large"
        type="submit"
        variant="contained"
        sx={{ mt: 3 }}
        loading={loading}
      >
        {isEditMode ? 'Update User' : 'Add User'}
      </LoadingButton>

      {submissionError && (
        <Typography color="error" sx={{ mt: 2 }}>
          {submissionError}
        </Typography>
      )}

      {loading && (
        <Box
          sx={{
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            bottom: 0,
            backgroundColor: 'rgba(0, 0, 0, 0.5)',
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            zIndex: 1000,
          }}
        >
          <CircularProgress />
        </Box>
      )}
    </Box>
  );
}