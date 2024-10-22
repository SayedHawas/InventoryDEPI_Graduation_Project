import { Controller, Control, FieldValues, Path, RegisterOptions } from 'react-hook-form';
import { MuiTelInput } from 'mui-tel-input';
import { FormControl, FormHelperText } from '@mui/material';

interface FormPhoneNumberFieldProps<TFieldValues extends FieldValues> {
  name: Path<TFieldValues>;
  control: Control<TFieldValues>;
  label?: string;
  rules?: RegisterOptions<TFieldValues>;
  error?: boolean;
  helperText?: string;
}

export const FormPhoneNumberField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  rules,
  error,
  helperText,
}: FormPhoneNumberFieldProps<TFieldValues>) => {
  return (
    <Controller<TFieldValues>
      name={name}
      control={control}
      rules={{
        ...rules,
        validate: (value) => {
          if (!value) return true;
          
          const trimmedValue = value.replace(/\s+/g, '');
          console.log(value,trimmedValue)
          const isValidLength = trimmedValue.length === 13;
          const isValidPrefix =
            value.startsWith('+20 10') ||
            value.startsWith('+20 11') ||
            value.startsWith('+20 12') ||
            value.startsWith('+20 15');

          if (!isValidLength) {
            return 'Phone number must be 10 digits long, excluding +20.';
          }
          if (!isValidPrefix) {
            return 'Phone number must start with 010, 011, 012, or 015.';
          }

          return true;
        },
      }}
      render={({ field }) => (
        <FormControl fullWidth error={error} margin="normal">
          <MuiTelInput
            {...field}
            
            langOfCountryName='EG'
            onlyCountries={['EG']}
            defaultCountry="EG"
            fullWidth
            margin="normal"
            label={label}
            onChange={(newValue) => {
              field.onChange(newValue);
            }}
          />
          {helperText && <FormHelperText>{helperText}</FormHelperText>}
        </FormControl>
      )}
    />
  );
};
