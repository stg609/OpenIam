import { PlusOutlined } from '@ant-design/icons';
import ProForm, { DrawerForm, ModalForm, ProFormSelect, ProFormSwitch, ProFormText, ProFormTextArea } from '@ant-design/pro-form';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumns } from '@ant-design/pro-table';
import type { ActionType as ProListActionType } from '@ant-design/pro-list/node_modules/@ant-design/pro-table';
import type { ReactText } from 'react';
import type { PermissionDto } from '../Permission/data';
import ProList from '@ant-design/pro-list';
import ProTable from '@ant-design/pro-table';
import ProCard from '@ant-design/pro-card';
import { Modal, Space, Button, Table, notification } from 'antd';
import React, { useRef, useState } from 'react';
import { useModel } from 'umi';
import * as PermissionApi from '../../services/permission';
import * as ClientApi from '../../services/client';
import * as RoleApi from '../../services/role';

type RoleListItem = API.RoleDto;

export default (): React.ReactNode => {
    const { initialState } = useModel('@@initialState');
    const [ownedPermRowKeys, setOwnedPermRowKeys] = useState<ReactText[]>([]);
    const [unownedPermRowKeys, setUnOwnedPermRowKeys] = useState<ReactText[]>([]);
    const actionRef = useRef<ActionType>();
    const ownedActionRef = useRef<ProListActionType>();
    const unownedActionRef = useRef<ProListActionType>();
    const accessToken = initialState?.currentUser?.accessToken ?? "";

    const handleBatchDeleteAsync = async (selectedKeys: React.ReactText[]) => {
        return await RoleApi.batchDeleteAsync(accessToken, selectedKeys.map(itm => itm.toString()));
    }

    const ownedPermSelection = {
        selectedRowKeys: ownedPermRowKeys,
        onChange: (keys: ReactText[]) => setOwnedPermRowKeys(keys),
    };
    const unownedPermSelection = {
        selectedRowKeys: unownedPermRowKeys,
        onChange: (keys: ReactText[]) => setUnOwnedPermRowKeys(keys),
    };
    const columns: ProColumns<RoleListItem>[] = [
        {
            title: '名称',
            dataIndex: 'name',
        },
        {
            title: '客户端 Id',
            dataIndex: 'clientId',
        },
        {
            title: '描述',
            hideInSearch: true,
            dataIndex: 'desc',
        },
        {
            title: '是否是超级管理员',
            hideInSearch: true,
            dataIndex: 'isSuperAdmin',
            render: (_, record) => (
                record.isSuperAdmin ? "是" : "否"
            )
        },
        {
            title: '是否是管理员',
            hideInSearch: true,
            dataIndex: 'isAdmin',
            render: (_, record) => (
                record.isAdmin ? "是" : "否"
            )
        },
        {
            title: '创建时间',
            hideInSearch: true,
            dataIndex: 'createdAt',
            valueType: 'dateTime',
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
                            await RoleApi.updateAsync(accessToken, record.id, value as RoleListItem);
                            if (actionRef.current) {
                                actionRef.current.reload();
                            }
                            notification.success({ message: "操作成功！" });
                            return true;
                        }
                        catch (ex) {
                            notification.error({ message: "更新角色时遇到错误", description: ex.message });
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
                        label="名称"
                        placeholder="请输入名称"
                    />
                    <ProFormTextArea
                        width="md"
                        name="desc"
                        label="描述"
                        placeholder="请输入描述"
                    />
                    <ProFormSwitch
                        width="md"
                        name="isAdmin"
                        label="是否是管理员"
                    />
                    <ProFormSwitch
                        width="md"
                        name="isSuperAdmin"
                        label="是否是超级管理员"
                        disabled
                    />
                </DrawerForm>,
                <ModalForm key="permissions"
                    submitter={{
                        submitButtonProps: {
                            style: {
                                display: 'none',
                            },
                        },
                    }}
                    trigger={
                        <a>权限管理</a>
                    }>
                    <ProCard >
                        <ProCard bordered>
                            <ProList<PermissionDto> style={{ maxHeight: '500px', overflow: 'auto' }}
                                actionRef={ownedActionRef}
                                size='small'
                                split={true}
                                headerTitle="已经拥有的权限"
                                rowKey="id"
                                request={async () => {
                                    try {
                                        const result = await RoleApi.getPermsAsync(accessToken, record.id);
                                        return result;
                                    }
                                    catch (ex) {
                                        notification.error({ message: "获取已拥有的权限时遇到错误！", description: ex.message });
                                        return [];
                                    }
                                }}
                                metas={{
                                    title: {
                                        dataIndex: 'name',
                                        title: '名称'
                                    }
                                }}
                                rowSelection={ownedPermSelection}
                            />
                        </ProCard>
                        <ProCard
                            layout="center"
                            colSpan={{ sm: 4 }}
                            direction="column"
                        >
                            <ProCard type="inner" ghost={false} >
                                <Button onClick={async () => {
                                    try {
                                        await RoleApi.removePermsAsync(accessToken, record.id, ownedPermRowKeys.map(itm => itm.toString()));
                                        ownedActionRef.current?.reload()
                                        unownedActionRef.current?.reload()
                                        setOwnedPermRowKeys([]);
                                        notification.success({ message: '删除权限成功！' });
                                    }
                                    catch (ex) {
                                        notification.error({ message: '删除权限时遇到错误', description: ex.message });
                                    }
                                }}>{'>>'}</Button>
                            </ProCard>
                            <ProCard type="inner" >
                                <Button onClick={async () => {
                                    try {
                                        await RoleApi.addPermsAsync(accessToken, record.id, unownedPermRowKeys.map(itm => itm.toString()));
                                        ownedActionRef.current?.reload()
                                        unownedActionRef.current?.reload()
                                        setUnOwnedPermRowKeys([]);
                                        notification.success({ message: '权限添加成功！' });
                                    }
                                    catch (ex) {
                                        notification.error({ message: '添加权限时遇到错误', description: ex.message });
                                    }
                                }}>{'<<'}</Button>
                            </ProCard>
                        </ProCard>
                        <ProCard bordered>
                            <ProList<PermissionDto> style={{ maxHeight: '500px', overflow: 'auto' }}
                                actionRef={unownedActionRef}
                                size='small'
                                split={true}
                                headerTitle="未拥有的权限"
                                rowKey="id"
                                request={async () => {
                                    try {
                                        const result = await PermissionApi.queryAsync({ access_token: accessToken, excludeRoleId: record.id });
                                        return result;
                                    }
                                    catch (ex) {
                                        notification.error({ message: '获取未拥有的权限时遇到错误', description: ex.message });
                                        return []
                                    }
                                }}
                                metas={{
                                    title: {
                                        dataIndex: 'name',
                                        title: '名称'
                                    }
                                }}
                                rowSelection={unownedPermSelection}
                            />
                        </ProCard>
                    </ProCard>

                </ModalForm>
            ],
        }
    ];

    return (
        <PageContainer>
            <ProTable<RoleListItem>
                rowKey="id"
                columns={columns}
                actionRef={actionRef}
                request={async (params) => {
                    try {
                        const roles = await RoleApi.queryAsync({ access_token: accessToken, name: params.name, clientId: params.clientId, pageIndex: params.current, pageSize: params.pageSize });
                        return roles;
                    }
                    catch (ex) {
                        notification.error({ message: "获取角色列表时遇到错误", description: ex.message });
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
                                            notification.success({ message: "操作成功！" });
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
                        width="380px"
                        title="新建角色"
                        trigger={
                            <Button type="primary">
                                <PlusOutlined />
                                新建角色
                            </Button>
                        }
                        onFinish={async (value) => {
                            try {
                                await RoleApi.createAsync(accessToken, value as RoleListItem);
                                if (actionRef.current) {
                                    actionRef.current.reload();
                                }
                                notification.success({ message: "角色创建成功！" });
                                return true;
                            }
                            catch (ex) {
                                notification.error({ message: "创建角色时遇到错误！", description: ex.message });
                                return false;
                            }
                        }}
                    >
                        <ProForm.Group>
                            <ProFormSelect
                                width="md"
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
                            <ProFormText
                                width="md"
                                name="name"
                                label="名称"
                                placeholder="请输入名称"
                                rules={[
                                    {
                                        required: true,
                                        message: '请输入名称',
                                    }
                                ]}
                            />
                        </ProForm.Group>

                        <ProForm.Group>
                            <ProFormSwitch
                                width="md"
                                name="isAdmin"
                                label="是否是管理员"
                            />
                        </ProForm.Group>
                        <ProFormTextArea
                            name="desc"
                            label="描述"
                            placeholder="请输入描述"
                        />
                    </ModalForm>
                ]}
            />

        </PageContainer>
    )
};
