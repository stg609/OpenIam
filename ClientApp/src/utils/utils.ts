import { notification } from 'antd';
import { UserManager } from 'oidc-client';
import { RequestError } from './RequestError';
import * as IamSettings from '../../config/iamSettings';

/* eslint no-useless-escape:0 import/prefer-default-export:0 */
const reg = /(((^https?:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+(?::\d+)?|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)$/;

export const isUrl = (path: string): boolean => reg.test(path);

export const isAntDesignPro = (): boolean => {
  if (ANT_DESIGN_PRO_ONLY_DO_NOT_USE_IN_YOUR_PRODUCTION === 'site') {
    return true;
  }
  return window.location.hostname === 'preview.pro.ant.design';
};

// 给官方演示站点用，用于关闭真实开发环境不需要使用的特性
export const isAntDesignProOrDev = (): boolean => {
  const { NODE_ENV } = process.env;
  if (NODE_ENV === 'development') {
    return true;
  }
  return isAntDesignPro();
};

export const sleep = (ms: number) => {
  return new Promise(resolve => setTimeout(resolve, ms));
}

export const ThinRequestAsync = async (params: {
  url: string,
  access_token?: string,
  type?: 'json' | 'form',
  method?: string,
  body?: BodyInit
}) => {
  const requestHeaders: HeadersInit = new Headers();
  if (params.access_token) {
    requestHeaders.set('Authorization', `Bearer ${params.access_token}`);
  }

  let reqType = params.type;
  if (!reqType) {
    reqType = 'json';
  }

  switch (reqType) {
    case 'form':
      requestHeaders.set('Accept', `application/json`);
      requestHeaders.set('Content-Type', `application/x-www-form-urlencoded;charset=UTF-8`);
      break;
    case 'json':
    default:
      requestHeaders.set('Accept', `application/json`);
      requestHeaders.set('Content-Type', `application/json;charset=UTF-8`);
      break;
  }

  try {
    const resp = await fetch(params.url, {
      headers: requestHeaders,
      method: params.method,
      body: params.body
    });

    if (resp.ok) {
      return resp;
    }

    throw new RequestError(resp.status, resp.statusText);
  }
  catch (ex) {

    if (ex instanceof RequestError) {
      if (ex.Status === 401) {
        notification.error({
          message: `您尚未登录，或登录已经超时， 正在重新为您跳转到登录页面`,
          description: `${ex.message}`,
        });

        // 说明没有登录，则进行登录操作
        const userManager = new UserManager(IamSettings.default);
        await userManager.signinRedirect();
        await sleep(10000);
      }
    }

    throw ex;
  }
}

