import React from 'react';
import { Autocomplete, TextField, FormHelperText, FormControl } from '@mui/material';
import { Controller, Control, FieldValues, Path, RegisterOptions, FieldPath } from 'react-hook-form';

interface SelectOption {
    value: string;
    label: string;
}

interface AutocompleteMultiSelectFieldProps<TFieldValues extends FieldValues> {
    name: Path<TFieldValues>;
    control: Control<TFieldValues>;
    label?: string;
    options: SelectOption[] | (() => Promise<SelectOption[]>);
    rules?: Omit<RegisterOptions<TFieldValues, FieldPath<TFieldValues>>, 'valueAsNumber' | 'valueAsDate' | 'setValueAs' | 'disabled'>;
    error?: boolean;
    helperText?: string;
    autocompleteProps?: Omit<React.ComponentProps<typeof Autocomplete>, 'options' | 'onChange' | 'renderInput'>;
}

export const AutocompleteMultiSelectField = <TFieldValues extends FieldValues>({
    name,
    control,
    label,
    options,
    rules,
    error,
    helperText,
    autocompleteProps,
}: AutocompleteMultiSelectFieldProps<TFieldValues>) => {
    const [selectOptions, setSelectOptions] = React.useState<SelectOption[]>([]);

    React.useEffect(() => {
        const loadOptions = async () => {
            if (typeof options === 'function') {
                const fetchedOptions = await options();
                setSelectOptions(fetchedOptions);
            } else {
                setSelectOptions(options);
            }
        };

        loadOptions();
    }, [options]);

    const isOptionEqualToValue = (option: any, value: any) => {
        return option.value === value.value;
    }

    // Function to map form values to SelectOption objects
    const mapSelectedValuesToOptions = (values: string[]): SelectOption[] => {
        return values
            .map(value => selectOptions.find(option => option.value === value))
            .filter((option): option is SelectOption => option !== undefined); // Filter out undefined
    }

    return (
        <Controller<TFieldValues>
            name={name}
            control={control}
            rules={rules}
            render={({ field }) => (
                <FormControl fullWidth error={error} margin="normal">
                    <Autocomplete
                        isOptionEqualToValue={isOptionEqualToValue}
                        multiple
                        options={selectOptions}
                        // Use field.value directly and map it to SelectOption objects
                        value={field.value ? mapSelectedValuesToOptions(field.value) : []}  
                        getOptionLabel={(option: any) => option.label}
                        onChange={(event, value: any) => {
                            field.onChange(value.map((v: { value: any; }) => v.value)); // Store the raw values (just the value)
                        }}
                        renderInput={(params) => (
                            <TextField
                                {...params}
                                label={label}
                                error={error}
                            />
                        )}
                        {...autocompleteProps}
                    />
                    {helperText && <FormHelperText>{helperText}</FormHelperText>}
                </FormControl>
            )}
        />
    );
};
