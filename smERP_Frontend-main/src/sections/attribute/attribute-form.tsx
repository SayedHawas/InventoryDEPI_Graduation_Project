import { LoadingButton } from "@mui/lab";
import { Box, Typography, CircularProgress, Button, IconButton } from "@mui/material";
import { useState, useEffect } from "react";
import { SubmitHandler, useForm, useFieldArray, FieldValues, Control } from "react-hook-form";
import { FormField } from "src/components/form-fields/form-field";
import { Iconify } from "src/components/iconify";
import { apiService } from "src/services/api";

interface AttributeValueData {
    attributeValueId: string;
    englishName: string;
    arabicName: string;
}

export interface AttributeFormData {
    attributeId?: string;
    englishName: string;
    arabicName: string;
    valuesToEdit: AttributeValueData[];
    valuesToAdd: Omit<AttributeValueData, 'attributeValueId'>[];
}

interface AttributeFormProps {
    attributeId?: string;
    onSubmitSuccess: () => void;
}

export function AttributeForm({ attributeId, onSubmitSuccess }: AttributeFormProps) {
    const [loading, setLoading] = useState(false);
    const [submissionError, setSubmissionError] = useState<string | null>(null);
    const [fetchingAttribute, setFetchingAttribute] = useState(false);
    const isEditMode = !!attributeId;

    const { control, handleSubmit, reset, formState: { errors, dirtyFields } } = useForm<AttributeFormData>({
        defaultValues: {
            attributeId: '',
            englishName: '',
            arabicName: '',
            valuesToEdit: [],
            valuesToAdd: []
        },
    });

    const { fields: valuesToAddFields, append, remove } = useFieldArray({
        control,
        name: "valuesToAdd",
    });

    useEffect(() => {
        const fetchAttributeData = async () => {
            if (attributeId) {
                setFetchingAttribute(true);
                try {
                    const response = await fetch(`https://smerp.runasp.net/attributes/${attributeId}`);
                    if (!response.ok) {
                        throw new Error('Failed to fetch attribute data');
                    }
                    const responseBody = await response.json();
                    const attributeData = responseBody.value;

                    reset({
                        attributeId: attributeData.attributeId,
                        englishName: attributeData.englishName,
                        arabicName: attributeData.arabicName,
                        valuesToEdit: attributeData.values,
                        valuesToAdd: []
                    });
                } catch (error) {
                    console.error('Error fetching attribute data:', error);
                    setSubmissionError('Failed to load attribute data. Please try again.');
                } finally {
                    setFetchingAttribute(false);
                }
            }
        };

        fetchAttributeData();
    }, [attributeId, reset]);

    const onSubmit: SubmitHandler<AttributeFormData> = async (data) => {
        setLoading(true);
        setSubmissionError(null);
        try {
            if (isEditMode) {
                const changedData = getChangedValues(dirtyFields as DirtyFields<AttributeFormData>, data);
                const requestBody = {
                    ...changedData,
                    attributeId: attributeId
                };
                await apiService.attributes.update(attributeId, requestBody);
            } else {
                await apiService.attributes.create(data);
            }
            console.log(isEditMode ? 'Attribute updated successfully' : 'Attribute added successfully');
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
            <FormField<AttributeFormData>
                name="englishName"
                control={control}
                label="English Name"
                rules={{ required: 'English name is required' }}
                error={!!errors.englishName}
                helperText={errors.englishName?.message}
            />

            <FormField<AttributeFormData>
                name="arabicName"
                control={control}
                label="Arabic Name"
                rules={{ required: 'Arabic name is required' }}
                error={!!errors.arabicName}
                helperText={errors.arabicName?.message}
            />
            
            <ValuesToEditFields control={control} />

            <Typography variant="h6" sx={{ mt: 3, mb: 2 }}>New Attribute Values</Typography>

            {valuesToAddFields.map((field, index) => (
                <Box key={field.id} sx={{ display: 'flex', gap: 2, mb: 2, alignItems: 'center' }}>
                    <FormField<AttributeFormData>
                        name={`valuesToAdd.${index}.englishName`}
                        control={control}
                        label="English Name"
                        rules={{ required: 'English name is required' }}
                        error={!!errors.valuesToAdd?.[index]?.englishName}
                        helperText={errors.valuesToAdd?.[index]?.englishName?.message}
                    />
                    <FormField<AttributeFormData>
                        name={`valuesToAdd.${index}.arabicName`}
                        control={control}
                        label="Arabic Name"
                        rules={{ required: 'Arabic name is required' }}
                        error={!!errors.valuesToAdd?.[index]?.arabicName}
                        helperText={errors.valuesToAdd?.[index]?.arabicName?.message}
                    />
                    <IconButton style={{ height: 'fit-content' }} onClick={() => remove(index)}>
                        <Iconify icon="mingcute:close-fill" />
                    </IconButton>
                </Box>
            ))}

            <Button
                variant="outlined"
                onClick={() => append({ englishName: '', arabicName: '' })}
                sx={{ mt: 1, mb: 3 }}
            >
                Add Value
            </Button>

            <LoadingButton
                fullWidth
                size="large"
                type="submit"
                variant="contained"
                sx={{ mt: 3 }}
                loading={loading}
            >
                {isEditMode ? 'Update Attribute' : 'Add Attribute'}
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
    );
}

interface ValuesToEditFieldsProps {
    control: Control<AttributeFormData>;
}

function ValuesToEditFields({ control }: ValuesToEditFieldsProps) {
    const { fields } = useFieldArray({
        control,
        name: "valuesToEdit",
    });

    return (
        <>
            {fields.length > 0 && <Typography variant="h6" sx={{ mt: 3, mb: 2 }}>Existing Attribute Values</Typography>}

            {fields.map((field, index) => (
                <Box key={field.id} sx={{ display: 'flex', gap: 2, mb: 2 }}>
                    <FormField<AttributeFormData>
                        name={`valuesToEdit.${index}.englishName`}
                        control={control}
                        label="English Name"
                        rules={{ required: 'English name is required' }}
                    />
                    <FormField<AttributeFormData>
                        name={`valuesToEdit.${index}.arabicName`}
                        control={control}
                        label="Arabic Name"
                        rules={{ required: 'Arabic name is required' }}
                    />
                </Box>
            ))}
        </>
    );
}

type DeepPartial<T> = {
    [P in keyof T]?: T[P] extends (infer U)[]
    ? DeepPartial<U>[]
    : T[P] extends object
    ? DeepPartial<T[P]>
    : T[P];
};

type DirtyFields<T> = {
    [K in keyof T]?: T[K] extends (infer U)[]
    ? DirtyFields<U>[]
    : T[K] extends object
    ? DirtyFields<T[K]>
    : boolean;
};

function getChangedValues<T extends Record<string, any>>(
    dirtyFields: DirtyFields<T>,
    allValues: T
): DeepPartial<T> {
    const changedValues: DeepPartial<T> = {};
    Object.keys(dirtyFields).forEach((key) => {
        const typedKey = key as keyof T;
        if (dirtyFields[typedKey] === true) {
            changedValues[typedKey] = allValues[typedKey];
        } else if (Array.isArray(dirtyFields[typedKey])) {
            if (typedKey === 'valuesToEdit') {
                changedValues[typedKey] = (dirtyFields[typedKey] as DirtyFields<T[keyof T]>[])
                    .map((subDirtyFields, index) => {
                        const subChangedValues = getChangedValues(subDirtyFields, (allValues[typedKey] as any)[index]);
                        if (Object.keys(subChangedValues).length > 0) {
                            return {
                                attributeValueId: (allValues[typedKey] as AttributeValueData[])[index].attributeValueId,
                                ...subChangedValues
                            };
                        }
                        return null;
                    })
                    .filter((subChangedValues): subChangedValues is NonNullable<typeof subChangedValues> =>
                        subChangedValues !== null
                    ) as any;
            } else {
                changedValues[typedKey] = (dirtyFields[typedKey] as DirtyFields<T[keyof T]>[])
                    .map((subDirtyFields, index) =>
                        getChangedValues(subDirtyFields, (allValues[typedKey] as any)[index])
                    )
                    .filter((subChangedValues) => Object.keys(subChangedValues).length > 0) as any;
            }
        } else if (typeof dirtyFields[typedKey] === 'object') {
            changedValues[typedKey] = getChangedValues(
                dirtyFields[typedKey] as DirtyFields<T[keyof T]>,
                allValues[typedKey] as any
            ) as any;
        }
    });
    return changedValues;
}