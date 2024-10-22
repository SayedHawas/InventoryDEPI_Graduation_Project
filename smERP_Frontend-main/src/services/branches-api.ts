export const fetchBranchesList = async () => {
    const response = await fetch('https://api.example.com/user');
    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.message || 'Failed to fetch user data');
    }
    return response.json();
  };
