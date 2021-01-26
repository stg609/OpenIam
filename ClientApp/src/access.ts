// src/access.ts
export default function access(initialState: { currentUser?: API.CurrentUser | undefined }) {
  const { currentUser } = initialState || {};
  return {
    isAdmin: currentUser != null && currentUser.isAdmin,
    isSuperAdmin: currentUser!= null && currentUser.isSuperAdmin
  };
}
