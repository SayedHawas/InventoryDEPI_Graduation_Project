import { Address } from "src/services/types";
import { apiService } from "src/services/api";
import { useState, useEffect } from "react";
import { SubmitHandler, useFieldArray, useForm } from "react-hook-form";
import { LoadingButton } from "@mui/lab";
import { Box, Typography, CircularProgress, Button } from "@mui/material";
import { FormField } from "src/components/form-fields/form-field";
import { AddressFieldGroup } from "src/components/form-fields/address-form-group";

export interface SupplierFormData {
    supplierId?: string,
    englishName: string,
    arabicName: string,
    addresses: Address[]
}

interface SupplierFormProps {
    supplierId?: string,
    onSubmitSuccess: () => void;
}

export function SupplierForm({ supplierId, onSubmitSuccess }: SupplierFormProps) {
    const [loading, setLoading] = useState(false);
    const [submissionError, setSubmissionError] = useState<string | null>(null);
    const [fetchingSupplier, setFetchingSupplier] = useState(false);
    const isEditMode = !!supplierId;

    const { control, handleSubmit, reset, formState: { errors, dirtyFields } } = useForm<SupplierFormData>({
        defaultValues: {
            supplierId: '',
            englishName: '',
            arabicName: '',
            addresses: [{}]
        },
    });

    const { fields, append, remove } = useFieldArray({
        control,
        name: "addresses"
    });

    useEffect(() => {
        const fetchSupplierData = async () => {
            if (supplierId) {
                setFetchingSupplier(true);
                const response = await apiService.suppliers.getById(Number(supplierId))
                if (!response.isSuccess) {
                    setSubmissionError('Failed to load supplier data. Please try again.');
                }
                else {
                    reset(response.value)
                }
                setFetchingSupplier(false);
            }
        };

        fetchSupplierData();
    }, [supplierId, reset]);

    const onSubmit: SubmitHandler<SupplierFormData> = async (data) => {
        setLoading(true);
        setSubmissionError(null);
        try {
            if (isEditMode) {
                const changedData = Object.fromEntries(
                    Object.entries(dirtyFields)
                      .filter(([_, isDirty]) => isDirty)
                      .map(([key]) => [key, data[key as keyof SupplierFormData]])
                  ) as Partial<SupplierFormData>;
                const requestBody = {
                    ...changedData,
                    supplierId: supplierId
                };
                await apiService.suppliers.update(supplierId, requestBody);
            } else {
                await apiService.suppliers.create(data);
            }
            console.log(isEditMode ? 'Supplier updated successfully' : 'Supplier added successfully');
            onSubmitSuccess();
        } catch (error: any) {
            console.error(error);
            setSubmissionError(error.message || "An unexpected error occurred. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Box component="form" autoComplete="off" onSubmit={handleSubmit(onSubmit)}>
            <FormField<SupplierFormData>
                name="englishName"
                control={control}
                label="English Name"
                rules={{ required: 'English name is required' }}
                error={!!errors.englishName}
                helperText={errors.englishName?.message}
            />

            <FormField<SupplierFormData>
                name="arabicName"
                control={control}
                label="Arabic Name"
                rules={{ required: 'Arabic name is required' }}
                error={!!errors.arabicName}
                helperText={errors.arabicName?.message}
            />

            <Typography variant="subtitle1" gutterBottom sx={{ mt: 2 }}>
                Addresses
            </Typography>
            
            {fields.map((field, index) => (
                <Box key={field.id} sx={{ mb: 2 }}>
                    <AddressFieldGroup<SupplierFormData>
                        control={control}
                        prefix={`addresses.${index}`}
                        errors={errors.addresses?.[index]}
                    />
                    <Button color="error" onClick={() => remove(index)} disabled={fields.length === 1}>
                        Remove Address
                    </Button>
                </Box>
            ))}

            <Button variant="contained" onClick={() => append({country: '', city: '', state: '', street: '', postalCode: '', comment: ''})} sx={{ mb: 2 }}>
                Add Address
            </Button>

            {submissionError && (
                <Typography color="error" sx={{ mt: 2 }}>
                    {submissionError}
                </Typography>
            )}

            <LoadingButton
                fullWidth
                size="large"
                type="submit"
                variant="contained"
                sx={{ mt: 3 }}
                loading={loading}
            >
                {isEditMode ? 'Update Supplier' : 'Add Supplier'}
            </LoadingButton>

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
    )
}