import React from 'react';
import { TextField, InputAdornment, IconButton, TextFieldProps } from '@mui/material';
import { Controller, Control, FieldValues, Path, RegisterOptions, FieldPath } from 'react-hook-form';
import { Iconify } from 'src/components/iconify';

interface FormFieldProps<TFieldValues extends FieldValues> {
  name: Path<TFieldValues>;
  control: Control<TFieldValues>;
  label: string;
  type?: string;
  rules?: Omit<RegisterOptions<TFieldValues, FieldPath<TFieldValues>>, 'valueAsNumber' | 'valueAsDate' | 'setValueAs' | 'disabled'>;
  showPasswordToggle?: boolean;
  error?: boolean;
  helperText?: string;
  textFieldProps?: Omit<TextFieldProps, 'name' | 'control' | 'label' | 'type' | 'error' | 'helperText'>;
  render?: (field: any) => React.ReactNode;
  isNumber?: boolean;
  isCurrency?: boolean;
  isReadOnly?: boolean;
  onChange?: (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => void;
}

export const FormField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  type = 'text',
  rules,
  showPasswordToggle = false,
  error,
  helperText,
  textFieldProps,
  render,
  isNumber = false,
  isCurrency = false,
  isReadOnly = false,
  onChange
}: FormFieldProps<TFieldValues>) => {
  const [showPassword, setShowPassword] = React.useState(false);

  const handleNumericInput = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    let value = event.target.value;
    
    if (isCurrency) {
      value = value.replace(/[^\d.]/g, '');
      const parts = value.split('.');
      if (parts.length > 2) {
        parts.pop();
        value = parts.join('.');
      }
      if (parts[1] && parts[1].length > 2) {
        parts[1] = parts[1].substring(0, 2);
        value = parts.join('.');
      }
    } else if (isNumber) {
      value = value.replace(/\D/g, '');
    }
    
    event.target.value = value;
    return event;
  };

  const getInputAdornment = () => {
    if (isCurrency) {
      return (
        <InputAdornment position="start">
          <Iconify icon="mingcute:currency-euro-2-line" />
        </InputAdornment>
      );
    }
    return undefined;
  };

  return (
    <Controller<TFieldValues>
      name={name}
      control={control}
      rules={rules}
      render={({ field }) => (
        <>
          {render ? (
            render(field)
          ) : (
            <TextField
              disabled={isReadOnly}
              fullWidth
              margin="normal"
              label={label}
              type={showPasswordToggle && showPassword ? 'text' : type}
              {...field}
              {...textFieldProps}
              error={error}
              helperText={helperText}
              InputProps={{
                startAdornment: getInputAdornment(),
                endAdornment: showPasswordToggle ? (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() => setShowPassword((prev) => !prev)}
                      edge="end"
                    >
                      <Iconify
                        icon={showPassword ? 'solar:eye-bold' : 'solar:eye-closed-bold'}
                      />
                    </IconButton>
                  </InputAdornment>
                ) : undefined,
                onChange: (event) => {
                  const processedEvent = (isNumber || isCurrency) ? handleNumericInput(event) : event;
                  field.onChange(processedEvent.target.value);
                  if (onChange) onChange(processedEvent);
                },
                ...(textFieldProps?.InputProps || {}),
              }}
            />
          )}
        </>
      )}
    />
  );
};