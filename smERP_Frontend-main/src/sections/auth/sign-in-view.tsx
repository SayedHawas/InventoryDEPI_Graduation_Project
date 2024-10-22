import React from 'react';
import { useForm, SubmitHandler } from 'react-hook-form';
import { Box, Typography } from '@mui/material';
import { LoadingButton } from '@mui/lab';
import { useRouter } from 'src/routes/hooks';
import { FormField } from 'src/components/form-fields/form-field';
import { useAuth } from 'src/contexts/AuthContext';

interface SignInFormValues {
  email: string;
  password: string;
}

export function SignInView() {
  const router = useRouter();
  const { login, isLoading } = useAuth();
  const { control, handleSubmit, formState: { errors } } = useForm<SignInFormValues>({
    defaultValues: {
      email: 'o@gmail.com',
      password: '123456zZ!',
    },
  });

  const onSubmit: SubmitHandler<SignInFormValues> = async (data) => {
    try {
      await login(data.email, data.password);
      router.push('/');
    } catch (error) {
      console.error('Login failed', error);
    }
  };

  return (
    <Box
      component="form"
      onSubmit={handleSubmit(onSubmit)}
      display="flex"
      flexDirection="column"
      alignItems="stretch"
    >
      <FormField<SignInFormValues>
        name="email"
        control={control}
        label="Email address"
        rules={{
          required: 'Email is required',
          pattern: {
            value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
            message: "Invalid email address"
          }
        }}
        error={!!errors.email}
        helperText={errors.email?.message}
      />
      <FormField<SignInFormValues>
        name="password"
        control={control}
        label="Password"
        type="password"
        showPasswordToggle
        rules={{
          required: 'Password is required',
          minLength: {
            value: 6,
            message: 'Password must be at least 6 characters'
          }
        }}
        error={!!errors.password}
        helperText={errors.password?.message}
      />
      <LoadingButton
        fullWidth
        size="large"
        type="submit"
        color="inherit"
        variant="contained"
        loading={isLoading}
        sx={{ mt: 2 }}
      >
        Sign in
      </LoadingButton>
      {(errors.email || errors.password) && (
        <Typography color="error" sx={{ mt: 2 }}>
          Please correct the errors above to sign in.
        </Typography>
      )}
    </Box>
  );
}