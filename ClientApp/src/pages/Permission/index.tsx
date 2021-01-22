import { PlusOutlined } from '@ant-design/icons';
import ProForm, { DrawerForm, ModalForm, ProFormDependency, ProFormDigit, ProFormRadio, ProFormSelect, ProFormText, ProFormTextArea } from '@ant-design/pro-form';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumns } from '@ant-design/pro-table';
import ProTable from '@ant-design/pro-table';
import { Modal, Space, Button, Table, notification } from 'antd';
import React, { useRef } from 'react';
import { useModel } from 'umi';
import * as PermissionApi from '../../services/permission';
import * as ClientApi from '../../services/client';
import type { PermissionDto } from './data';

type PermissionListItem = PermissionDto & { access_token: string };

export default (): React.ReactNode => {
    const { initialState } = useModel('@@initialState');
    const actionRef = useRef<ActionType>();
    const accessToken = initialState?.currentUser?.accessToken ?? "";

    const handleBatchDeleteAsync = async (selectedKeys: React.ReactText[]) => {
        return await PermissionApi.batchDeleteAsync(accessToken, selectedKeys.map(itm => itm.toString()));
    }

    const columns: ProColumns<PermissionListItem>[] = [
        {
            title: '名称',
            dataIndex: 'name',
        },
        {
            title: '客户端 Id',
            dataIndex: 'clientId',
        },
        {
            title: 'Key',
            dataIndex: 'key',
        },
        {
            title: '描述',
            hideInSearch: true,
            dataIndex: 'desc',
        },
        {
            title: '类型',
            dataIndex: 'type',
            initialValue: '-1',
            filters: true,
            onFilter: true,
            valueType: 'select',
            valueEnum: {
                "-1": { text: '全部' },
                "0": { text: '菜单' },
                "1": { text: 'Api' }
            },
        },
        {
            title: '排序',
            dataIndex: 'order',
            hideInSearch: true,
        },
        {
            title: '操作',
            fixed: 'right',
            valueType: 'option',
            render: (text, record) => [
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
                            await PermissionApi.updateAsync(accessToken, record.id, value as PermissionListItem);
                            if (actionRef.current) {
                                actionRef.current.reload();
                            }
                            notification.success({ message: `更新权限成功！` });
                            return true;
                        }
                        catch (ex) {
                            notification.error({ message: "更新权限时遇到错误！", description: ex.message });
                            return false;
                        }
                    }}
                >
                    <ProFormSelect
                        width="md"
                        name="clientId"
                        label="所属客户端"
                        readonly
                        request={async () => {
                            try {
                                const clients = await ClientApi.queryAsync(accessToken, null, null, 0, 0);
                                if (clients.data) {
                                    return clients.data.map(itm => ({ label: itm.clientName, value: itm.clientId }));
                                }

                                return [];
                            }
                            catch (ex) {
                                notification.error({ message: "获取客户端列表时遇到错误！", description: ex.message });
                                return [];
                            }
                        }
                        }
                        rules={[
                            {
                                required: true,
                                message: '所属客户端',
                            }
                        ]}
                    />
                    <ProFormText
                        width="md"
                        name="name"
                        label="权限名称"
                        placeholder="请输入名称"
                    />
                    <ProFormText
                        width="md"
                        name="key"
                        label="权限Key"
                        readonly
                        placeholder="权限Key"
                        rules={[
                            {
                                required: true,
                                message: '权限Key',
                            }
                        ]}
                    />
                    <ProFormRadio.Group
                        name="type"
                        label="菜单类型"
                        radioType="button"
                        options={[
                            {
                                label: '菜单',
                                value: 0,
                            },
                            {
                                label: 'Api',
                                value: 1,
                            }
                        ]}
                        rules={[
                            {
                                required: true,
                                message: '权限Key',
                            }
                        ]}
                    />
                    <ProFormSelect
                        width="md"
                        name="parentId"
                        label="上级权限"
                        request={async () => {
                            try {
                                const perms = await PermissionApi.queryAsync({ access_token: accessToken });
                                if (perms.data) {
                                    return perms.data.map(itm => ({ label: itm.name, value: itm.id }));
                                }

                                return [];
                            }
                            catch (ex) {
                                notification.error({ message: "获取上级权限时遇到错误！", description: ex.message });
                                return [];
                            }
                        }
                        }
                    />
                    <ProFormTextArea
                        width="md"
                        name="desc"
                        label="描述"
                        placeholder="请输入描述"
                    />

                    <ProFormDependency name={['type']}>
                        {({ type }) => {
                            if (type === 0) {
                                return (
                                    <ProFormText
                                        width="md"
                                        name="url"
                                        label="菜单地址"
                                        placeholder="请输入菜单地址"
                                        rules={[
                                            {
                                                required: true,
                                                message: '请输入菜单地址',
                                            }
                                        ]}
                                    />
                                );
                            }

                            return null;
                        }}
                    </ProFormDependency>
                    <ProFormDependency name={['type']}>
                        {({ type }) => {
                            if (type === 0) {
                                return (
                                    <ProFormDigit
                                        width="md"
                                        name="order"
                                        label="菜单顺序"
                                        placeholder="请输入菜单顺序"
                                    />
                                );
                            }

                            return null;
                        }}
                    </ProFormDependency>
                </DrawerForm>
            ],
        }
    ];

    return (
        <PageContainer>
            <ProTable<PermissionListItem>
                rowKey="id"
                columns={columns}
                actionRef={actionRef}
                request={async (params) => {
                    try {
                        const clients = await PermissionApi.queryAsync({
                            access_token: accessToken, name: params.name, key: params.key,
                            url: params.url, clientId: params.clientId, type: params.type
                        });
                        return { ...clients, data: clients.data.map(d => ({ ...d, access_token: accessToken })) };
                    }
                    catch (ex) {
                        notification.error({ message: "获取权限列表时遇到错误！", description: ex.message });
                        return [];
                    }
                }}
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
                                            notification.success({ message: `操作成功！` });
                                        }
                                        catch {
                                            notification.error({ message: "操作失败！" });
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
                        title="新建权限"
                        trigger={
                            <Button type="primary">
                                <PlusOutlined />
                                新建权限
                            </Button>
                        }
                        onFinish={async (value) => {
                            try {
                                await PermissionApi.createAsync(accessToken, value as PermissionListItem);
                                if (actionRef.current) {
                                    actionRef.current.reload();
                                }
                                notification.success({ message: "新建权限成功！" });
                                return true;
                            }
                            catch (ex) {
                                notification.error({ message: "新建权限时遇到错误！", description: ex.message });
                                return false;
                            }
                        }}
                    >
                        <ProFormSelect
                            name="clientId"
                            label="所属客户端"
                            request={async () => {
                                try {
                                    const clients = await ClientApi.queryAsync(accessToken, null, null, 0, 0);
                                    if (clients.data) {
                                        return clients.data.map(itm => ({ label: itm.clientName, value: itm.clientId }));
                                    }

                                    return [];
                                }
                                catch (ex) {
                                    notification.error({ message: "获取客户端列表时遇到错误！", description: ex.message });
                                    return [];
                                }
                            }
                            }
                            rules={[
                                {
                                    required: true,
                                    message: '所属客户端',
                                }
                            ]}
                        />
                        <ProForm.Group>
                            <ProFormText
                                width="md"
                                name="name"
                                label="权限名称"
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
                                name="key"
                                label="权限Key"
                                placeholder="权限Key"
                                rules={[
                                    {
                                        required: true,
                                        message: '权限Key',
                                    }
                                ]}
                            />
                        </ProForm.Group>
                        <ProForm.Group>
                            <ProFormRadio.Group
                                width="md"
                                name="type"
                                label="菜单类型"
                                radioType="button"
                                initialValue={1}
                                options={[
                                    {
                                        label: '菜单',
                                        value: 0,
                                    },
                                    {
                                        label: 'Api',
                                        value: 1,
                                    }
                                ]}
                                rules={[
                                    {
                                        required: true,
                                        message: '权限Key',
                                    }
                                ]}
                            />
                            <ProFormSelect
                                width="md"
                                name="parentId"
                                label="上级权限"
                                request={async () => {
                                    try {
                                        const perms = await PermissionApi.queryAsync({ access_token: accessToken });
                                        if (perms.data) {
                                            return perms.data.map(itm => ({ label: itm.name, value: itm.id }));
                                        }

                                        return [];
                                    }
                                    catch (ex) {
                                        notification.error({ message: "获取上级权限时遇到错误！", description: ex.message });
                                        return [];
                                    }
                                }
                                }
                            />
                        </ProForm.Group>
                        <ProFormTextArea
                            name="desc"
                            label="描述"
                            placeholder="请输入描述"
                        />

                        <ProForm.Group>
                            <ProFormDependency name={['type']}>
                                {({ type }) => {
                                    if (type === 0) {
                                        return (
                                            <ProFormText
                                                width="md"
                                                name="url"
                                                label="菜单地址"
                                                placeholder="请输入菜单地址"
                                                rules={[
                                                    {
                                                        required: true,
                                                        message: '请输入菜单地址',
                                                    }
                                                ]}
                                            />
                                        );
                                    }

                                    return null;
                                }}
                            </ProFormDependency>
                            <ProFormDependency name={['type']}>
                                {({ type }) => {
                                    if (type === 0) {
                                        return (
                                            <ProFormDigit
                                                width="md"
                                                name="order"
                                                label="菜单顺序"
                                                initialValue={0}
                                                placeholder="请输入菜单顺序"
                                                fieldProps={{ precision: 0 }}
                                            />
                                        );
                                    }

                                    return null;
                                }}
                            </ProFormDependency>
                        </ProForm.Group>

                    </ModalForm>
                ]}
            />

        </PageContainer>
    )
};
