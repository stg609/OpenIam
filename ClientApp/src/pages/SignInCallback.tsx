import React, { useEffect } from 'react';
import { UserManager } from 'oidc-client';
import { history, useModel } from 'umi';
import { queryCurrentPermissionsAsync } from '../services/user';
import { PageLoading } from '@ant-design/pro-layout';

export default (): React.ReactNode => {
    const { initialState, setInitialState } = useModel('@@initialState');
    useEffect(() => {
        new UserManager({ response_mode: "query" }).signinRedirectCallback().then(async user => {
            if (user != null) {
                const permObj = await queryCurrentPermissionsAsync(user.access_token);
                const userObj: API.CurrentUser = {
                    accessToken: user?.access_token,
                    idToken: user?.id_token,
                    name: user?.profile.name,
                    avatar: user?.profile.picture,
                    userid: user?.profile.sub,
                    isSuperAdmin: permObj?.isSuperAdmin ?? false,
                    isAdmin: permObj?.isAdmin ?? false,
                };
                setInitialState({ ...initialState, currentUser: userObj })
                history.push("/welcome");
            }
        }).catch(e => {
            console.error(e);
        });
    });

    return <PageLoading />;
};
