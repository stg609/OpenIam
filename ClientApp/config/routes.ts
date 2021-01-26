export default [  
  {
    path: '/welcome',
    name: 'welcome',
    icon: 'smile',
    component: './Welcome',
    access: 'isAdmin',
    hideInMenu: false
  },
  {
    path: '/iamadmin',
    name: 'iam-admin',
    icon: 'crown',
    access: 'isAdmin',
    routes: [
      {
        path: '/iamadmin/client',
        name: 'client',
        icon: 'smile',
        component: './Client'
      },
      {
        path: '/iamadmin/permission',
        name: 'permission',
        icon: 'smile',
        component: './Permission'
      },
      {
        path: '/iamadmin/role',
        name: 'role',
        icon: 'smile',
        component: './Role'
      },
      {
        path: '/iamadmin/org',
        name: 'org',
        icon: 'smile',
        component: './Org'
      },
      {
        path: '/iamadmin/user',
        name: 'user',
        icon: 'smile',
        component: './User'
      },
    ],
  },
  {
    path: '/admin',
    name: 'admin',
    icon: 'crown',
    access: 'isSuperAdmin',
    routes: [
      {
        path: '/admin/sys',
        name: 'sys',
        icon: 'smile',
        component: './Sys'
      },
    ]
  },  
  {
    path:'/signin',
    component: './SignInCallback'
  },
  {
    path:'/silent-renew',
    component: './SilentRenewCallback'
  },
  {
    path:'/signout-callback-oidc',
    component: './SignoutRedirectCallback'
  },
  {
    path: '/error',
    component: './error',
  },
  {
    path: '/',
    redirect: '/Welcome',
  },
  {
    component: './404',
  }
];
