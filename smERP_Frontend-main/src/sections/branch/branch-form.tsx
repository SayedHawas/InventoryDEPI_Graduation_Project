import { LoadingButton } from "@mui/lab";
import { Box, Typography, CircularProgress } from "@mui/material";
import { useEffect, useState } from "react";
import { SubmitHandler, useForm } from "react-hook-form";
import { FormField } from "src/components/form-fields/form-field";
import { apiService } from "src/services/api";

export interface BranchFormData {
    branchId?: string,
    englishName: string,
    arabicName: string
}

interface BranchFormProps {
    branchId?: string;
    onSubmitSuccess: () => void;
}

export function BranchForm({ branchId, onSubmitSuccess }: BranchFormProps) {
    const [loading, setLoading] = useState(false);
    const [submissionError, setSubmissionError] = useState<string | null>(null);
    const [fetchingBranch, setFetchingBranch] = useState(false);
    const isEditMode = !!branchId;

    const { control, handleSubmit, reset, formState: { errors, dirtyFields } } = useForm<BranchFormData>({
        defaultValues: {
            branchId: '',
            englishName: '',
            arabicName: ''
        },
    });

    useEffect(() => {
        const fetchBranchData = async () => {
            if (branchId) {
                setFetchingBranch(true);
                try {
                    const response = await fetch(`https://smerp.runasp.net/branches/${branchId}`);
                    if (!response.ok) {
                        throw new Error('Failed to fetch brand data');
                    }
                    const responseBody = await response.json();
                    const brandData: BranchFormData = responseBody.value
                    reset(brandData);
                } catch (error) {
                    console.error('Error fetching brand data:', error);
                    setSubmissionError('Failed to load brand data. Please try again.');
                } finally {
                    setFetchingBranch(false);
                }
            }
        };

        fetchBranchData();
    }, [branchId, reset]);

    const onSubmit: SubmitHandler<BranchFormData> = async (data) => {
        setLoading(true);
        setSubmissionError(null);
        try {
            if (isEditMode) {
                const changedData = Object.keys(dirtyFields).reduce((acc: Partial<BranchFormData>, key) => {
                    acc[key as keyof BranchFormData] = data[key as keyof BranchFormData];
                    return acc;
                }, {});
                const requestBody = {
                    ...changedData,
                    branchId: branchId
                };
                await apiService.branches.update(branchId, requestBody);
            } else {
                await apiService.branches.create(data);
            }
            console.log(isEditMode ? 'Branch updated successfully' : 'Branch added successfully');
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
            <FormField<BranchFormData>
                name="englishName"
                control={control}
                label="English Name"
                rules={{ required: 'English name is required' }}
                error={!!errors.englishName}
                helperText={errors.englishName?.message}
            />

            <FormField<BranchFormData>
                name="arabicName"
                control={control}
                label="Arabic Name"
                rules={{ required: 'Arabic name is required' }}
                error={!!errors.arabicName}
                helperText={errors.arabicName?.message}
            />

            <LoadingButton
                fullWidth
                size="large"
                type="submit"
                variant="contained"
                sx={{ mt: 3 }}
                loading={loading}
            >
                {isEditMode ? 'Update Branch' : 'Add Branch'}
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