import { PlusOutlined } from '@ant-design/icons';
import ProForm, { DrawerForm, ModalForm, ProFormDigit, ProFormSwitch, ProFormText, ProFormTextArea } from '@ant-design/pro-form';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumns } from '@ant-design/pro-table';
import ProTable from '@ant-design/pro-table';
import { Modal, Space, Tag, Button, Table, notification } from 'antd';
import React, { useRef } from 'react';
import { useModel } from 'umi';
import * as ClientApi from '../../services/client';

type ClientListItem = API.ClientDto & { access_token: string };

export default (): React.ReactNode => {
    const { initialState } = useModel('@@initialState');
    const actionRef = useRef<ActionType>();
    const accessToken = initialState?.currentUser?.accessToken ?? "";

    const handleCreateClientAsync = async (fields: ClientListItem) => {
        const result = await ClientApi.createAsync(accessToken, fields);
        return result?.plainSecret;
    }

    const handleBatchDeleteAsync = async (selectedKeys: React.ReactText[]) => {
        return await ClientApi.batchDeleteAsync(accessToken, selectedKeys.map(itm => itm.toString()));
    }

    const columns: ProColumns<ClientListItem>[] = [
        {
            title: '名称',
            dataIndex: 'clientName'
        },
        {
            title: 'Client Id',
            dataIndex: 'clientId'
        },
        {
            title: '客户端地址',
            dataIndex: 'clientUri',
            hideInSearch: true,
            render: (_, record) => (
                <a href={record.clientUri}>跳转</a>
            )
        },
        {
            title: '允许的范围',
            hideInSearch: true,
            dataIndex: 'allowedScopes',
            render: (_, record) => (
                <Space>
                    {record.allowedScopes.map(scope => (<Tag key={scope}>{scope}</Tag>))}
                </Space>

            )
        },
        {
            title: '状态',
            hideInSearch: true,
            dataIndex: 'isEnabled',
            render: (_, record) => (
                record.isEnabled ? "已启用" : "未启用"
            )
        },
        {
            title: '操作',
            fixed: 'right',
            valueType: 'option',
            render: (text, record, _, action) => [
                <DrawerForm width="380px" key="detail"
                    title="详情"
                    initialValues={record}
                    trigger={
                        <a key="details">
                            详情
                        </a>
                    }
                    onFinish={async (value) => {
                        try {
                            await ClientApi.updateAsync(accessToken, record.clientId, value as ClientListItem);
                            if (actionRef.current) {
                                actionRef.current.reload();
                            }
                            notification.success({ message: "客户端更新成功！" });
                            return true;
                        }
                        catch (ex) {
                            notification.error({ message: "客户端更新失败！", description: ex.message });
                            return false;
                        }
                    }}
                >
                    <ProFormText
                        width="md"
                        name="clientName"
                        label="客户端名称"
                        placeholder="请输入名称"
                        rules={[
                            {
                                required: true,
                                message: '请输入名称',
                            }
                        ]}
                    />
                    <ProFormText
                        width="md"
                        name="clientUri"
                        label="地址"
                        placeholder="请输入客户端地址"
                        rules={[
                            {
                                required: true,
                                message: '请输入客户端地址',
                            }
                        ]}
                    />
                    <ProFormTextArea
                        width="md"
                        name="description"
                        label="描述"
                        placeholder="请输入描述"
                    />

                    <ProFormText
                        width="md"
                        name="redirectUri"
                        label="回调地址"
                        placeholder="请输入回调地址，多个地址用英文逗号分隔"
                    />
                    <ProFormText
                        width="md"
                        name="allowedScopes"
                        label="允许的 Scope"
                        placeholder="请输入允许的 Scope，多个Scope用英文逗号分隔"
                    />
                    <ProFormText
                        width="md"
                        name="AllowedCorsOrigins"
                        label="允许的 跨域地址"
                        placeholder="请输入允许的 跨域地址，多个跨域地址用英文逗号分隔"
                    />
                    <ProFormDigit
                        width="md"
                        name="accessTokenLifetime"
                        min={1}
                        fieldProps={{ precision: 0 }}
                        label="Acess Token 生命周期（秒)"
                    />
                    <ProFormDigit
                        width="md"
                        name="identityTokenLifetime"
                        min={1}
                        fieldProps={{ precision: 0 }}
                        label="Id Token 生命周期（秒)"
                    />
                    <ProFormSwitch
                        width="md"
                        name="isEnabled"
                        label="是否启用"
                    />
                </DrawerForm>,
                <a key="enable"
                    onClick={() => {
                        Modal.confirm({
                            title: '操作',
                            content: `确认要${record.isEnabled ? "禁用" : "启用"}吗？`,
                            okText: '确认',
                            cancelText: '取消',
                            onOk: async () => {
                                try {
                                    await ClientApi.enableAsync(record.access_token, record.clientId, !record.isEnabled);
                                    notification.success({ message: `${record.isEnabled ? "禁用" : "启用"}成功！` });
                                    action.reload();
                                    return true;
                                }
                                catch (ex) {
                                    notification.error({ message: `${record.isEnabled ? "禁用" : "启用"}遇到错误！`, description: ex.message });
                                    return false;
                                }

                            }
                        });
                    }}
                >
                    {record.isEnabled ? "禁用" : "启用"}
                </a >,
                <a key="refreshSecrets"
                    onClick={() => {
                        Modal.confirm({
                            title: '操作',
                            content: `确认要刷新密钥吗？`,
                            okText: '确认',
                            cancelText: '取消',
                            onOk: async () => {
                                try {
                                    const secret = await ClientApi.refreshSecretsAsync(record.access_token, record.clientId);
                                    notification.success({ message: `密钥刷新成功`, description: `新的密钥：${secret}，请妥善保管！` });
                                    return true;
                                }
                                catch (ex) {
                                    notification.error({ message: "密钥刷新失败！", description: ex.message });
                                    return false;
                                }
                            }
                        });
                    }}
                >
                    刷新密钥
                </a>
            ],
        }
    ];

    return (
        <PageContainer>
            <ProTable<ClientListItem>
                rowKey="clientId"
                columns={columns}
                actionRef={actionRef}
                request={async (params) => {
                    try {
                        const clients = await ClientApi.queryAsync(accessToken, params.clientName, params.clientId, params.current, params.pageSize);
                        return { ...clients, data: clients.data.map(d => ({ ...d, access_token: accessToken })) };
                    }
                    catch (ex) {
                        notification.error({ message: "获取客户端列表时遇到错误！", description: ex.message });
                        return [];
                    }
                }}
                pagination={{ pageSize: 10 }}
                rowSelection={{
                    // 自定义选择项参考: https://ant.design/components/table-cn/#components-table-demo-row-selection-custom
                    // 注释该行则默认不显示下拉选项
                    selections: [Table.SELECTION_ALL, Table.SELECTION_INVERT],
                }}
                tableAlertOptionRender={({ selectedRowKeys, onCleanSelected }) => {
                    return (
                        <Space size={16}>
                            <a onClick={async () => {
                                Modal.confirm({
                                    title: '操作',
                                    content: `确认要删除吗？`,
                                    okText: '确认',
                                    cancelText: '取消',
                                    onOk: async () => {
                                        try {
                                            await handleBatchDeleteAsync(selectedRowKeys);
                                            if (actionRef.current) {
                                                actionRef.current.reload();
                                            }
                                            onCleanSelected();
                                            notification.success({ message: `删除成功！` });
                                            return true;
                                        }
                                        catch (ex) {
                                            notification.error({ message: "删除失败！", description: ex.message });
                                            return false;
                                        }
                                    }
                                });

                            }}>批量删除</a>
                        </Space>
                    );
                }}
                toolBarRender={() => [
                    <ModalForm
                        width="740px"
                        title="新建客户端"
                        trigger={
                            <Button type="primary">
                                <PlusOutlined />
                                新建客户端
                            </Button>
                        }
                        onFinish={async (value) => {
                            try {
                                await handleCreateClientAsync(value as ClientListItem);
                                if (actionRef.current) {
                                    actionRef.current.reload();
                                }
                                notification.success({ message: `客户端新建成功！` });
                                return true;
                            }
                            catch (ex) {
                                notification.error({ message: "客户端新建失败！", description: ex.message });
                                return false;
                            }
                        }}
                    >
                        <ProForm.Group>
                            <ProFormText
                                width="md"
                                name="clientName"
                                label="客户端名称"
                                placeholder="请输入名称"
                                rules={[
                                    {
                                        required: true,
                                        message: '请输入名称',
                                    }
                                ]}
                            />
                            <ProFormText
                                width="md"
                                name="clientUri"
                                label="地址"
                                placeholder="请输入客户端地址"
                                rules={[
                                    {
                                        required: true,
                                        message: '请输入客户端地址',
                                    }
                                ]}
                            />
                        </ProForm.Group>
                        <ProFormText
                            name="redirectUri"
                            label="回调地址"
                            placeholder="请输入回调地址，多个地址用英文逗号分隔"
                            rules={[
                                {
                                    required: true,
                                    message: '请输入用于 OAuth2 登录的回调地址',
                                }
                            ]}
                        />
                        <ProFormTextArea
                            name="description"
                            label="描述"
                            placeholder="请输入描述"
                        />


                        <ProForm.Group>
                            <ProFormText
                                width="md"
                                name="allowedScopes"
                                label="允许的 Scope"
                                placeholder="请输入允许的 Scope，多个Scope用英文逗号分隔"
                            />
                            <ProFormText
                                width="md"
                                name="AllowedCorsOrigins"
                                label="允许的 跨域地址"
                                placeholder="请输入允许的 跨域地址，多个跨域地址用英文逗号分隔"
                            />
                        </ProForm.Group>
                        <ProForm.Group>
                            <ProFormDigit
                                width="md"
                                name="accessTokenLifetime"
                                min={1}
                                fieldProps={{ precision: 0 }}
                                label="Acess Token 生命周期（秒)"
                                initialValue={3600}
                            />
                            <ProFormDigit
                                width="md"
                                name="identityTokenLifetime"
                                min={1}
                                fieldProps={{ precision: 0 }}
                                label="Id Token 生命周期（秒)"
                                initialValue={3600}
                            />
                        </ProForm.Group>
                    </ModalForm>
                ]}
            />

        </PageContainer>
    )
};
