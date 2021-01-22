// src/access.ts
export default function access(initialState: { currentUser?: API.CurrentUser | undefined }) {
  const { currentUser } = initialState || {};
  return {
    isAdmin: currentUser && currentUser.isAdmin,
    isSuperAdmin: currentUser && currentUser.isSuperAdmin
  };
}
