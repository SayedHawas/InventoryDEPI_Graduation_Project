import { LoadingButton } from "@mui/lab";
import { Box, Typography, CircularProgress } from "@mui/material";
import { useEffect, useState } from "react";
import { SubmitHandler, useForm } from "react-hook-form";
import { FormField } from "src/components/form-fields/form-field";
import { FormSelectField } from "src/components/form-fields/form-select-field";
import { useParentCategories } from "src/hooks/use-categories";
import { apiService } from "src/services/api";

export interface CategoryFormData {
    categoryId?: string,
    englishName: string,
    arabicName: string,
    parentCategoryId?: string
}

interface CategoryFormProps {
    categoryId?: string;
    onSubmitSuccess: () => void;
}
export function CategoryForm({ categoryId, onSubmitSuccess }: CategoryFormProps) {
    const [loading, setLoading] = useState(false);
    const [submissionError, setSubmissionError] = useState<string | null>(null);
    const [fetchingCategory, setFetchingCategory] = useState(false);
    const isEditMode = !!categoryId;

    const { control, handleSubmit, reset, formState: { errors, dirtyFields } } = useForm<CategoryFormData>({
        defaultValues: {
            categoryId: '',
            englishName: '',
            arabicName: '',
            parentCategoryId: '',
        },
    });

    useEffect(() => {
        const fetchCategoryData = async () => {
            if (categoryId) {
                setFetchingCategory(true);
                try {
                    const response = await fetch(`https://smerp.runasp.net/categories/${categoryId}`);
                    if (!response.ok) {
                        throw new Error('Failed to fetch category data');
                    }
                    const responseBody = await response.json();
                    console.log(responseBody)
                    const categoryData: CategoryFormData = responseBody.value
                    reset(categoryData);
                } catch (error) {
                    console.error('Error fetching category data:', error);
                    setSubmissionError('Failed to load category data. Please try again.');
                } finally {
                    setFetchingCategory(false);
                }
            }
        };

        fetchCategoryData();
    }, [categoryId, reset]);

    const onSubmit: SubmitHandler<CategoryFormData> = async (data) => {
        setLoading(true);
        setSubmissionError(null);
        try {
            if (isEditMode) {
                const changedData = Object.keys(dirtyFields).reduce((acc: Partial<CategoryFormData>, key) => {
                    acc[key as keyof CategoryFormData] = data[key as keyof CategoryFormData];
                    return acc;
                }, {});
                const requestBody = {
                    ...changedData,
                    categoryId: categoryId
                };
                await apiService.categories.update(categoryId, requestBody);
            } else {
                await apiService.categories.create(data);
            }
            console.log(isEditMode ? 'Category updated successfully' : 'Category added successfully');
            onSubmitSuccess();
        } catch (error: any) {
            console.error(error);
            setSubmissionError(error.message || "An unexpected error occurred. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    const { data: categoriesResponse, error: categoriesError, isLoading: isLoadingCategories } = useParentCategories();

    return (
        <Box component="form" autoComplete="on" onSubmit={handleSubmit(onSubmit)}>
            <FormField<CategoryFormData>
                name="englishName"
                control={control}
                label="English Name"
                rules={{ required: 'English name is required' }}
                error={!!errors.englishName}
                helperText={errors.englishName?.message}
            />

            <FormField<CategoryFormData>
                name="arabicName"
                control={control}
                label="Arabic Name"
                rules={{ required: 'Arabic name is required' }}
                error={!!errors.arabicName}
                helperText={errors.arabicName?.message}
            />

            <FormSelectField<CategoryFormData>
                name="parentCategoryId"
                control={control}
                label="Parent Category"
                options={categoriesResponse?.value ?? []}
                error={!!errors.parentCategoryId}
                helperText={errors.parentCategoryId?.message}
            />

            <LoadingButton
                fullWidth
                size="large"
                type="submit"
                variant="contained"
                sx={{ mt: 3 }}
                loading={loading}
            >
                {isEditMode ? 'Update Category' : 'Add Category'}
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