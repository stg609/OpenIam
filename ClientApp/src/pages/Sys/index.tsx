import ProCard from '@ant-design/pro-card';
import ProForm, { ProFormSwitch } from '@ant-design/pro-form';
import { PageContainer, PageLoading } from '@ant-design/pro-layout';
import { notification } from 'antd';
import React, { useEffect, useState } from 'react';
import { useModel } from 'umi';
import * as SysApi from '../../services/sys';
import type { SysDto } from './data';

export default (): React.ReactNode => {
    const { initialState } = useModel('@@initialState');
    const [isLoading, setLoading] = useState(true);
    const [initialValue, setInitialValue] = useState<SysDto>({
        isJobNoPwdLoginEnabled: false, isJobNoUnique: false,
        isPhonePwdLoginEnabled: false, isRegisterUserEnabled: false,
        isUserPhoneUnique: false, lastUpdatedAt: ''
    });
    const accessToken = initialState?.currentUser?.accessToken ?? "";

    useEffect(() => {
        const request = async () => {
            try {
                const result = await SysApi.queryAsync(accessToken);
                setInitialValue({ ...result });
            }
            catch (ex) {
                notification.error({ message: "获取系统信息时遇到错误！", description: ex.message });
            }
            finally {
                setLoading(false);
            }
        };

        request();
    }, [accessToken])

    return (
        <PageContainer>
            {isLoading ?
                <PageLoading /> :
                <ProCard>
                    <ProForm
                        initialValues={initialValue}
                        onFinish={async (values) => {
                            try {
                                await SysApi.updateAsync(accessToken, values as SysDto);

                                notification.success({ message: "系统配置更新成功！" });
                                return true;
                            }
                            catch (ex) {
                                notification.error({ message: "系统配置更新失败！", description: ex.message });
                                return false;
                            }
                        }}>
                        <ProForm.Group>
                            <ProFormSwitch
                                width="md"
                                name="isUserPhoneUnique"
                                label="手机号是否唯一"
                            />
                            <ProFormSwitch
                                width="md"
                                name="isPhonePwdLoginEnabled"
                                label="是否开启手机号登录"
                            />
                        </ProForm.Group>
                        <ProForm.Group>
                            <ProFormSwitch
                                width="md"
                                name="isJobNoUnique"
                                label="工号是否唯一"
                            />
                            <ProFormSwitch
                                width="md"
                                name="isJobNoPwdLoginEnabled"
                                label="是否开启工号登录"
                            />
                        </ProForm.Group>
                        <ProForm.Group>
                            <ProFormSwitch
                                width="md"
                                name="isRegisterUserEnabled"
                                label="是否允许用户注册"
                            />
                        </ProForm.Group>
                    </ProForm>
                </ProCard>
            }
        </PageContainer >
    )
};
