import React, { useState, useCallback } from 'react';
import { useDropzone, FileRejection, DropzoneOptions } from 'react-dropzone';
import { Controller, Control, FieldValues, Path, RegisterOptions, FieldPath } from 'react-hook-form';
import { Box, Typography, IconButton, CircularProgress, Grid, Alert } from '@mui/material';
import { Iconify } from '../iconify';
import { ApiFile } from 'src/services/types';

interface ImageUploadFieldProps<TFieldValues extends FieldValues> {
  name: Path<TFieldValues>;
  control: Control<TFieldValues>;
  label: string;
  rules?: Omit<RegisterOptions<TFieldValues, FieldPath<TFieldValues>>, 'valueAsNumber' | 'valueAsDate' | 'setValueAs' | 'disabled'>;
  maxSize?: number;
  acceptedFileTypes?: string[];
  existingImages?: string[];
  error?: boolean;
  helperText?: string;
  onRemove?: (removedUrl: string) => void;
  maxImages?: number;
}

export const ImageUploadField = <TFieldValues extends FieldValues>({
  name,
  control,
  label,
  rules,
  maxSize = 10 * 1024 * 1024,
  acceptedFileTypes = ['image/jpeg', 'image/png', 'image/gif'],
  existingImages = [],
  error,
  helperText,
  onRemove,
  maxImages = 3,
}: ImageUploadFieldProps<TFieldValues>) => {
  const [images, setImages] = useState<ApiFile[]>(existingImages.map(url => ({ isNew: false, url: url })));
  const [loading, setLoading] = useState(false);
  const [limitReached, setLimitReached] = useState(false);
  const [duplicateFound, setDuplicateFound] = useState(false); // New state for duplicate checking

  const onDrop = useCallback(
    (acceptedFiles: File[], fileRejections: FileRejection[], onChange: (value: string[]) => void) => {
      if (fileRejections.length > 0) {
        console.error('File rejected:', fileRejections);
        return;
      }

      if (images.length + acceptedFiles.length > maxImages) {
        setLimitReached(true);
        return;
      }

      setLoading(true);
      setDuplicateFound(false); // Reset duplicate found state
      acceptedFiles.forEach((file) => {
        const reader = new FileReader();
        reader.onloadend = () => {
          const base64String = reader.result as string;

          // Check for duplicate images
          const isDuplicate = images.some(img => img.base64String === base64String);
          if (isDuplicate) {
            setDuplicateFound(true); // Set state if duplicate is found
            setLoading(false);
            return;
          }

          const newImage: ApiFile = { isNew: true, base64String: base64String };
          setImages((prevImages) => [...prevImages, newImage]);
          onChange(getNewImagesBase64(images.concat(newImage)));
          setLoading(false);
        };
        reader.readAsDataURL(file);
      });
    },
    [images, maxImages]
  );

  const removeImage = (image: ApiFile, onChange: (value: string[]) => void) => {
    let updatedImages: ApiFile[] = []
    if (image.url){
      updatedImages = images.filter((img) => img.url !== image.url);
    }
    else{
      updatedImages = images.filter((img) => img.base64String !== image.base64String);
    }
    setImages(updatedImages);
    onChange(getNewImagesBase64(updatedImages));
    if (onRemove && image.url) {
      onRemove(image.url);
    }
    setLimitReached(false);
    setDuplicateFound(false); // Reset duplicate found state on remove
  };

  const getNewImagesBase64 = (images: ApiFile[]): string[] => {
    return images.filter(img => img.isNew).map(img => img.base64String!);
  };

  return (
    <Controller<TFieldValues>
      name={name}
      control={control}
      rules={rules}
      render={({ field: { onChange, value } }) => {
        const dropzoneOptions: DropzoneOptions = {
          onDrop: (acceptedFiles, fileRejections) => onDrop(acceptedFiles, fileRejections, onChange),
          maxSize: maxSize,
          accept: acceptedFileTypes.reduce((acc, curr) => ({ ...acc, [curr]: [] }), {}),
          disabled: images.length >= maxImages,
        };

        const { getRootProps, getInputProps } = useDropzone(dropzoneOptions);

        return (
          <Box sx={{ mt: 2 }}>
            <Typography variant="subtitle1" gutterBottom>
              {label}
            </Typography>
            {images.length < maxImages && (
              <Box
                sx={{
                  border: '2px dashed #ccc',
                  borderRadius: 2,
                  padding: 2,
                  textAlign: 'center',
                  cursor: 'pointer',
                  minHeight: 100,
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                }}
                {...getRootProps()}
              >
                <input {...getInputProps()} />
                <Iconify icon={'mingcute:upload-2-line'} />
                <Typography sx={{ ml: 2 }}>
                  Drag & drop images here, or click to select ({images.length}/{maxImages})
                </Typography>
              </Box>
            )}

            <Grid container spacing={2} sx={{ mt: 2 }}>
              {images.map((img, index) => (
                <Grid item xs={4} sm={3} md={2} key={index}>
                  <Box sx={{ position: 'relative' }}>
                    <img
                      src={img.url ?? img.base64String}
                      alt={`Preview ${index + 1}`}
                      style={{ width: '100%', height: '100px', objectFit: 'cover', borderRadius: '4px' }}
                    />
                    <IconButton
                      size="small"
                      onClick={() => removeImage(img, onChange)}
                      sx={{
                        position: 'absolute',
                        top: 2,
                        right: 2,
                        backgroundColor: 'rgba(0, 0, 0, 0.5)',
                        color: 'white',
                        '&:hover': {
                          backgroundColor: 'rgba(0, 0, 0, 0.7)',
                        },
                      }}
                    >
                      <Iconify icon={'mingcute:close-line'} />
                    </IconButton>
                  </Box>
                </Grid>
              ))}
            </Grid>

            {loading && (
              <Box display="flex" justifyContent="center" mt={2}>
                <CircularProgress />
              </Box>
            )}
            {limitReached && (
              <Alert severity="warning" sx={{ mt: 2 }}>
                Image limit reached. Please remove an image before adding a new one.
              </Alert>
            )}
            {duplicateFound && (
              <Alert severity="error" sx={{ mt: 2 }}>
                Duplicate image detected. Please choose a different image.
              </Alert>
            )}
            {error && helperText && (
              <Typography color="error" variant="body2" mt={1}>
                {helperText}
              </Typography>
            )}
          </Box>
        );
      }}
    />
  );
};

export default ImageUploadField
