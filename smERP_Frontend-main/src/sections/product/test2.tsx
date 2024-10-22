// import React, { useEffect, useCallback, useState } from 'react';
// import { Box, Button, Grid, IconButton, Typography } from '@mui/material';
// import { useWatch } from 'react-hook-form';
// import { FormSelectField } from 'src/components/form-fields/form-select-field';
// import { Iconify } from 'src/components/iconify';
// import { AttributeValue } from 'src/services/types';

// export interface Attribute {
//   value: number;
//   label: string;
//   values: { value: number; label: string }[];
// }

// interface AttributeSelectorProps {
//   attributes: Attribute[];
//   control: any; // Type can be refined if you have a specific type for control
//   onAttributesChange: (selectedAttributes: AttributeValue[]) => void;
//   append: (item: any) => void; // Add the append function type
//   remove: (index: number) => void; // Add the remove function type
// }

// export const AttributeSelector: React.FC<AttributeSelectorProps> = ({
//   attributes,
//   control,
//   onAttributesChange,
//   append,
//   remove
// }) => {
//   const watchFieldArray = useWatch({ control, name: 'attributes' });
//   const [error, setErrorState] = useState<string | null>(null);

//   const updateSelectedAttributes = useCallback(() => {
//     const selectedAttributeIds = watchFieldArray?.map((field: AttributeValue) => field?.attributeId.toString()) || [];

//     const uniqueIds = new Set(selectedAttributeIds);
//     if (uniqueIds.size !== selectedAttributeIds.length) {
//       setErrorState('Attributes must be unique. You have selected one or more duplicate attributes.');
//     } else {
//       setErrorState(null);
//     }
    
//     const selectedAttributes = watchFieldArray
//       ?.filter((field: any) => field?.attributeId && field?.attributeValueId)
//       .map((field: any) => ({
//         attributeId: field.attributeId,
//         attributeValueId: field.attributeValueId,
//       })) || [];
      
//     onAttributesChange(selectedAttributes);
//   }, [watchFieldArray, onAttributesChange]);

//   useEffect(() => {
//     updateSelectedAttributes();
//   }, [updateSelectedAttributes]);

//   const getValueOptions = (index: number) => {
//     const attributeId = watchFieldArray?.[index]?.attributeId;
//     if (!attributeId) return [];
//     const attribute = attributes.find(attr => attr.value.toString() === attributeId.toString());
//     return attribute ? attribute.values.map(val => ({ value: (val as any).value.toString(), label: (val as any).label })) : [];
//   };

//   return (
//     <Box>
//       {watchFieldArray.map((field, index) => (
//         <Grid key={field.id} sx={{ display: 'flex', gap: 2, mb: 2, alignItems: 'center' }}>
//           <FormSelectField
//             name={`attributes.${index}.attributeId`}
//             control={control}
//             label="Attribute"
//             options={attributes.map(attr => ({
//               value: attr.value.toString(),
//               label: attr.label
//             }))}
//             rules={{ required: 'Attribute is required' }}
//           />
//           <FormSelectField
//             name={`attributes.${index}.attributeValueId`}
//             control={control}
//             label="Value"
//             options={getValueOptions(index)}
//             rules={{ required: 'Value is required' }}
//           />
//           <IconButton onClick={() => remove(index)}>
//             <Iconify icon={'mingcute:delete-2-line'} />
//           </IconButton>
//         </Grid>
//       ))}
//       <Button onClick={() => append({ attributeId: '', attributeValueId: '' })} variant="contained" disabled={!!error}>
//         Add Attribute
//       </Button>
//       {error && (
//         <Typography color="error" sx={{ mt: 2 }}>
//           {error}
//         </Typography>
//       )}
//     </Box>
//   );
// };
