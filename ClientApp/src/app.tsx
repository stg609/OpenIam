import React from 'react';
import type { Settings as LayoutSettings } from '@ant-design/pro-layout';
import { PageLoading } from '@ant-design/pro-layout';
import type { RunTimeLayoutConfig } from 'umi';
import { history } from 'umi';
import RightContent from '@/components/RightContent';
import Footer from '@/components/Footer';
import { queryCurrentPermissionsAsync } from './services/user';
import defaultSettings from '../config/defaultSettings';
import { UserManager } from 'oidc-client';
import * as IamSettings from '../config/iamSettings';
import { sleep } from '@/utils/utils';

/**
 * 获取用户信息比较慢的时候会展示一个 loading
 */
export const initialStateConfig = {
  loading: <PageLoading />,
};

export async function getInitialState(): Promise<{
  settings?: LayoutSettings;
  currentUser?: API.CurrentUser;
  fetchUserInfo?: () => Promise<API.CurrentUser | undefined>;
}> {
  const userManager = new UserManager(IamSettings.default);

  const fetchUserInfo = async () => {
    const user = await userManager.getUser();

    if (user == null) {
      return undefined;
    }

    const permObj = await queryCurrentPermissionsAsync(user.access_token);
    return {
      accessToken: user?.access_token,
      idToken: user?.id_token,
      name: user?.profile.name,
      avatar: user?.profile.picture,
      userid: user?.profile.sub,
      isAdmin: permObj?.isAdmin ?? false,
      isSuperAdmin: permObj?.isSuperAdmin ?? false,
    };
  };

  // 如果是登录页面，不执行
  if (history.location.pathname !== '/identity/account/login' && history.location.pathname !== `/${IamSettings.SiginPathSegment}`) {
    try {
      const user = await fetchUserInfo();

      if (!user) {
        await userManager.signinRedirect();
        await sleep(20000);
        return {};
      }

      return {
        fetchUserInfo,
        currentUser: user,
        settings: defaultSettings,
      };
    }
    catch (ex) {
      history.push({
        pathname: "/error",
        state: { ex }
      });
      throw ex;
    }
  }

  return {
    fetchUserInfo,
    settings: defaultSettings,
  };
}

export const layout: RunTimeLayoutConfig = ({ initialState }) => {
  return {
    logo: null,
    rightContentRender: () => <RightContent />,
    disableContentMargin: false,
    footerRender: () => <Footer />,
    onPageChange: () => {
      if (!initialState?.currentUser) {
        // 用户未登录
      }
    },
    menuHeaderRender: undefined,
    ...initialState?.settings,
  };
};
