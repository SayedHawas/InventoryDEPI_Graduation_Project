import React from 'react';
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  styled,
} from '@mui/material';
import { Iconify } from 'src/components/iconify';
import { Scrollbar } from 'src/components/scrollbar';

const BootstrapDialog = styled(Dialog)(({ theme }) => ({
  '& .MuiDialogContent-root': {
    padding: theme.spacing(2),
  },
  '& .MuiDialogActions-root': {
    padding: theme.spacing(1),
  },
}));

interface CustomDialogProps {
  open: boolean;
  handleCancel: () => void;
  title: string;
  content: React.ReactNode;
  actionText?: string | undefined;
}

export function CustomDialog({
  open,
  handleCancel,
  title,
  content,
  actionText,
}: CustomDialogProps) {
  return (
    <BootstrapDialog
      PaperProps={{
        sx: { minWidth: '380px', width: '60%', maxWidth: '100%' },
      }}
      onClose={handleCancel}
      aria-labelledby="customized-dialog-title"
      open={open}
    >
      <DialogTitle sx={{ m: 0, p: 2 }} id="customized-dialog-title">
        {title}
      </DialogTitle>
      <IconButton
        aria-label="close"
        onClick={handleCancel}
        sx={{
          position: 'absolute',
          right: 8,
          top: 8,
          color: (theme) => theme.palette.grey[500],
        }}
      >
        <Iconify
          icon={'mingcute:close-line'}
        />
      </IconButton>
      <Scrollbar>
        <DialogContent>{content}</DialogContent>
      </Scrollbar>

      {actionText && (
        <DialogActions>
          <Button autoFocus onClick={handleCancel}>
            {actionText}
          </Button>
        </DialogActions>
      )}
    </BootstrapDialog>
  );

}