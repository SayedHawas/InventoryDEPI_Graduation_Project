import React from 'react';
import { Control, Path, FieldValues, FieldError } from 'react-hook-form';
import { Grid } from '@mui/material';
import { FormField } from './form-field';

interface AddressFields {
  country: string;
  city: string;
  state: string;
  street: string;
  postalCode?: string;
  comment?: string;
}

interface AddressFieldGroupProps<TFieldValues extends FieldValues> {
  control: Control<TFieldValues>;
  prefix: string;
  errors?: Partial<Record<keyof AddressFields, FieldError>>;
}

export const AddressFieldGroup = <TFieldValues extends FieldValues>({
  control,
  prefix,
  errors,
}: AddressFieldGroupProps<TFieldValues>) => {
  const createFieldName = (name: keyof AddressFields): Path<TFieldValues> =>
    `${prefix}.${name}` as Path<TFieldValues>;

  return (
    <Grid container spacing={2}>
      <Grid item xs={12} sm={6}>
        <FormField<TFieldValues>
          name={createFieldName('country')}
          control={control}
          label="Country"
          rules={{ required: 'Country is required' }}
          error={!!errors?.country}
          helperText={errors?.country?.message}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <FormField<TFieldValues>
          name={createFieldName('city')}
          control={control}
          label="City"
          rules={{ required: 'City is required' }}
          error={!!errors?.city}
          helperText={errors?.city?.message}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <FormField<TFieldValues>
          name={createFieldName('state')}
          control={control}
          label="State"
          rules={{ required: 'State is required' }}
          error={!!errors?.state}
          helperText={errors?.state?.message}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <FormField<TFieldValues>
          name={createFieldName('street')}
          control={control}
          label="Street"
          rules={{ required: 'Street is required' }}
          error={!!errors?.street}
          helperText={errors?.street?.message}
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <FormField<TFieldValues>
          name={createFieldName('postalCode')}
          control={control}
          label="Postal Code"
        />
      </Grid>
      <Grid item xs={12} sm={6}>
        <FormField<TFieldValues>
          name={createFieldName('comment')}
          control={control}
          label="Comment"
        />
      </Grid>
    </Grid>
  );
};