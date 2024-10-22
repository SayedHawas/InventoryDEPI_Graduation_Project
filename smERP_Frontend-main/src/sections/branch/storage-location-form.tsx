import { LoadingButton } from "@mui/lab";
import { Box, Typography, CircularProgress } from "@mui/material";
import { useState, useEffect } from "react";
import { SubmitHandler, useForm } from "react-hook-form";
import { FormField } from "src/components/form-fields/form-field";
import { apiService } from "src/services/api";

export interface StorageLocationFormData {
    branchId: string,
    storageLocationsId?: string,
    name: string
}

interface StorageLocationFormProps {
    branchId: string;
    storageLocationsId?: string,
    onSubmitSuccess: () => void;
}

export function StorageLocationForm({ branchId, storageLocationsId, onSubmitSuccess }: StorageLocationFormProps) {
    const [loading, setLoading] = useState(false);
    const [submissionError, setSubmissionError] = useState<string | null>(null);
    const [fetchingStorageLocation, setFetchingStorageLocation] = useState(false);
    const isEditMode = !!storageLocationsId;

    const { control, handleSubmit, reset, formState: { errors, dirtyFields } } = useForm<StorageLocationFormData>({
        defaultValues: {
            branchId: branchId,
            storageLocationsId: '',
            name: ''
        },
    });

    useEffect(() => {
        const fetchStorageLocationData = async () => {
            if (storageLocationsId) {
                setFetchingStorageLocation(true);
                const response = await apiService.storageLocations.getById(branchId, storageLocationsId)
                if (!response.isSuccess) {
                    setSubmissionError('Failed to load supplier data. Please try again.');
                }
                else {
                    reset(response.value)
                }
                setFetchingStorageLocation(false);
            }
        };

        fetchStorageLocationData();
    }, [storageLocationsId, reset]);

    const onSubmit: SubmitHandler<StorageLocationFormData> = async (data) => {
        setLoading(true);
        setSubmissionError(null);
        try {
            if (isEditMode) {
                const changedData = Object.fromEntries(
                    Object.entries(dirtyFields)
                      .filter(([_, isDirty]) => isDirty)
                      .map(([key]) => [key, data[key as keyof StorageLocationFormData]])
                  ) as Partial<StorageLocationFormData>;
                const requestBody = {
                    ...changedData,
                    branchId: branchId,
                    storageLocationsId: storageLocationsId
                };
                await apiService.storageLocations.update(branchId, storageLocationsId, requestBody);
            } else {
                await apiService.storageLocations.create(branchId, data);
            }
            console.log(isEditMode ? 'Storage Location updated successfully' : 'Storage Location added successfully');
            onSubmitSuccess();
        } catch (error: any) {
            console.error(error);
            setSubmissionError(error.message || "An unexpected error occurred. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Box component="form" autoComplete="on" onSubmit={handleSubmit(onSubmit)}>
            <FormField<StorageLocationFormData>
                name="name"
                control={control}
                label="Name"
                rules={{ required: 'Name is required' }}
                error={!!errors.name}
                helperText={errors.name?.message}
            />

            <LoadingButton
                fullWidth
                size="large"
                type="submit"
                variant="contained"
                sx={{ mt: 3 }}
                loading={loading}
            >
                {isEditMode ? 'Update Storage Location' : 'Add Storage Location'}
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
    )
}