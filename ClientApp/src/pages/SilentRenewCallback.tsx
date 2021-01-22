import React, { useEffect } from 'react';
import { UserManager } from 'oidc-client';
import { PageLoading } from '@ant-design/pro-layout';

export default (): React.ReactNode => {
    useEffect(() => {
        new UserManager({}).signinSilentCallback();
    });

    return <PageLoading />;
};
