import { LoadingButton } from "@mui/lab";
import { Box, Typography, CircularProgress } from "@mui/material";
import { useEffect, useState } from "react";
import { SubmitHandler, useForm } from "react-hook-form";
import { FormField } from "src/components/form-fields/form-field";
import { apiService } from "src/services/api";

export interface BrandFormData {
    brandId?: string,
    englishName: string,
    arabicName: string
}

interface BrandFormProps {
    brandId?: string;
    onSubmitSuccess: () => void;
}

export function BrandForm({ brandId, onSubmitSuccess }: BrandFormProps) {
    const [loading, setLoading] = useState(false);
    const [submissionError, setSubmissionError] = useState<string | null>(null);
    const [fetchingBrand, setFetchingBrand] = useState(false);
    const isEditMode = !!brandId;

    const { control, handleSubmit, reset, formState: { errors, dirtyFields } } = useForm<BrandFormData>({
        defaultValues: {
            brandId: '',
            englishName: '',
            arabicName: ''
        },
    });

    useEffect(() => {
        const fetchBrandData = async () => {
            if (brandId) {
                setFetchingBrand(true);
                try {
                    const response = await fetch(`https://smerp.runasp.net/brands/${brandId}`);
                    if (!response.ok) {
                        throw new Error('Failed to fetch brand data');
                    }
                    const responseBody = await response.json();
                    const brandData: BrandFormData = responseBody.value
                    reset(brandData);
                } catch (error) {
                    console.error('Error fetching brand data:', error);
                    setSubmissionError('Failed to load brand data. Please try again.');
                } finally {
                    setFetchingBrand(false);
                }
            }
        };

        fetchBrandData();
    }, [brandId, reset]);

    const onSubmit: SubmitHandler<BrandFormData> = async (data) => {
        setLoading(true);
        setSubmissionError(null);
        try {
            if (isEditMode) {
                const changedData = Object.keys(dirtyFields).reduce((acc: Partial<BrandFormData>, key) => {
                    acc[key as keyof BrandFormData] = data[key as keyof BrandFormData];
                    return acc;
                }, {});
                const requestBody = {
                    ...changedData,
                    brandId: brandId
                };
                await apiService.brands.update(brandId, requestBody);
            } else {
                await apiService.brands.create(data);
            }
            console.log(isEditMode ? 'Brand updated successfully' : 'Brand added successfully');
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
            <FormField<BrandFormData>
                name="englishName"
                control={control}
                label="English Name"
                rules={{ required: 'English name is required' }}
                error={!!errors.englishName}
                helperText={errors.englishName?.message}
            />

            <FormField<BrandFormData>
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
                {isEditMode ? 'Update Brand' : 'Add Brand'}
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