import React from 'react';
import { TextFieldProps } from '@mui/material';
import { DatePicker, DatePickerProps } from '@mui/x-date-pickers/DatePicker';
import { Controller, Control, FieldValues, Path, RegisterOptions, FieldPath } from 'react-hook-form';

interface DateFieldProps<TFieldValues extends FieldValues> {
  name?: Path<TFieldValues>;
  control?: Control<TFieldValues>;
  label: string;
  rules?: Omit<RegisterOptions<TFieldValues, FieldPath<TFieldValues>>, 'valueAsNumber' | 'valueAsDate' | 'setValueAs' | 'disabled'>;
  error?: boolean;
  helperText?: string;
  textFieldProps?: Omit<TextFieldProps, 'name' | 'control' | 'label' | 'type' | 'error' | 'helperText'>;
  isReadOnly?: boolean;
  defaultValue?: Date;
  maxDate?: Date;
  onChange?: (date: Date | null) => void;
}

export const FormDateField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  rules,
  error,
  helperText,
  textFieldProps,
  isReadOnly = false,
  defaultValue,
  maxDate,
  onChange
}: DateFieldProps<TFieldValues>) => {
  const [value, setValue] = React.useState<Date | null>(defaultValue || null);

  const handleChange = (date: Date | null) => {
    setValue(date);
    if (onChange) {
      onChange(date);
    }
  };

  const datePickerProps: Partial<DatePickerProps<Date>> = {
    label,
    disabled: isReadOnly,
    maxDate,
    slotProps: {
      textField: {
        ...textFieldProps,
        fullWidth: true,
        error,
        helperText,
      } as TextFieldProps,
    },
  };

  if (!name || !control) {
    return (
      <DatePicker
        {...datePickerProps}
        value={value}
        onChange={handleChange}
      />
    );
  }

  return (
    <Controller<TFieldValues>
      name={name}
      control={control}
      rules={rules}
      render={({ field }) => (
        <DatePicker
          {...datePickerProps}
          value={field.value}
          onChange={(date) => {
            field.onChange(date);
            if (onChange) {
              onChange(date);
            }
          }}
        />
      )}
    />
  );
};