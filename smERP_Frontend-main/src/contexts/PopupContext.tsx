import React, { createContext, CSSProperties, ReactNode, useCallback, useContext, useState, useRef, useEffect } from "react";

// Define the context type
interface PopupContextType {
  openPopup: (content: ReactNode, options?: PopupOptions) => void;
  closePopup: () => void;
  updatePopup: (options: Partial<PopupOptions>) => void;
}

// Define popup options
interface PopupOptions {
  closeOnOutsideClick?: boolean;
  closeOnEscape?: boolean;
  className?: string;
  style?: CSSProperties;
  onClose?: () => void;
  position?: 'center' | 'top' | 'bottom' | 'left' | 'right';
  animation?: 'fade' | 'slide' | 'scale';
  backdrop?: boolean;
  backdropClassName?: string;
  maxWidth?: string;
  maxHeight?: string;
  hasCloseBtn?: boolean;
  title?: string;
}

// Create the context with a default value
const PopupContext = createContext<PopupContextType | undefined>(undefined);

// Popup provider component
interface PopupProviderProps {
  children: ReactNode;
}

export const PopupProvider: React.FC<PopupProviderProps> = ({ children }) => {
  const [popupContent, setPopupContent] = useState<ReactNode | null>(null);
  const [popupOptions, setPopupOptions] = useState<PopupOptions>({});
  const popupRef = useRef<HTMLDivElement>(null);

  const openPopup = useCallback((content: ReactNode, options: PopupOptions = {}) => {
    setPopupContent(content);
    setPopupOptions({ hasCloseBtn: true, ...options }); // Set hasCloseBtn to true by default
  }, []);

  const closePopup = useCallback(() => {
    setPopupContent(null);
    setPopupOptions({});
    popupOptions.onClose?.();
  }, [popupOptions]);

  const updatePopup = useCallback((newOptions: Partial<PopupOptions>) => {
    setPopupOptions(prev => ({ ...prev, ...newOptions }));
  }, []);

  const handleOutsideClick = useCallback((e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === e.currentTarget && popupOptions.closeOnOutsideClick) {
      closePopup();
    }
  }, [closePopup, popupOptions.closeOnOutsideClick]);

  const handleKeyDown = useCallback((e: KeyboardEvent) => {
    if (e.key === 'Escape' && popupOptions.closeOnEscape) {
      closePopup();
    }
  }, [closePopup, popupOptions.closeOnEscape]);

  useEffect(() => {
    document.addEventListener('keydown', handleKeyDown);
    return () => document.removeEventListener('keydown', handleKeyDown);
  }, [handleKeyDown]);

  useEffect(() => {
    if (popupContent && popupRef.current) {
      popupRef.current.focus();
    }
  }, [popupContent]);

  const getPositionClass = () => {
    switch (popupOptions.position) {
      case 'top': return 'items-start';
      case 'bottom': return 'items-end';
      case 'left': return 'justify-start';
      case 'right': return 'justify-end';
      default: return 'items-center justify-center';
    }
  };

  const getAnimationClass = () => {
    switch (popupOptions.animation) {
      case 'fade': return 'animate-fade-in';
      case 'slide': return 'animate-slide-in';
      case 'scale': return 'animate-scale-in';
      default: return '';
    }
  };

  return (
    <PopupContext.Provider value={{ openPopup, closePopup, updatePopup }}>
      {children}
      {popupContent && (
        <div 
          className={`fixed inset-0 ${popupOptions.backdrop ? 'bg-black bg-opacity-50' : ''} 
            flex ${getPositionClass()} z-50 ${popupOptions.backdropClassName || ''}`}
          onClick={handleOutsideClick}
          role="dialog"
          aria-modal="true"
          aria-labelledby="popup-title"
        >
          <div 
            ref={popupRef}
            tabIndex={-1}
            className={`relative bg-slate-500 p-6 rounded-lg shadow-lg ${getAnimationClass()} 
              ${popupOptions.className || ''}`}
            style={{
              ...popupOptions.style,
              maxWidth: popupOptions.maxWidth,
              maxHeight: popupOptions.maxHeight,
              overflow: 'auto'
            }}
          >
            {popupOptions.hasCloseBtn && (
              <button
                onClick={closePopup}
                className="absolute rounded-full top-6 right-6 text-gray-500 hover:text-gray-700"
                aria-label="Close popup"
              >
                &times;
              </button>
            )}
            {popupOptions.title && (
              <h2 id="popup-title" className="text-lg font-bold mb-4">{popupOptions.title}</h2>
            )}
            <div className="popup-content">
              {popupContent}
            </div>
          </div>
        </div>
      )}
    </PopupContext.Provider>
  );
};

// Custom hook to use the popup
export const usePopup = (): PopupContextType => {
  const context = useContext(PopupContext);
  if (!context) {
    throw new Error('usePopup must be used within a PopupProvider');
  }
  return context;
};