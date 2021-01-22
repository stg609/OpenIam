import { PlusOutlined } from '@ant-design/icons';
import ProForm, { DrawerForm, ModalForm, ProFormSelect, ProFormText } from '@ant-design/pro-form';
import { PageContainer } from '@ant-design/pro-layout';
import type { ActionType, ProColumns } from '@ant-design/pro-table';
import type { ActionType as ProListActionType } from '@ant-design/pro-list/node_modules/@ant-design/pro-table';
import ProList from '@ant-design/pro-list';
import ProTable from '@ant-design/pro-table';
import ProCard from '@ant-design/pro-card';
import { Modal, Button, notification } from 'antd';
import type { ReactText} from 'react';
import React, { useRef, useState } from 'react';
import { useModel } from 'umi';
import * as PermissionApi from '../../services/permission';
import * as RoleApi from '../../services/role';
import * as UserApi from '../../services/user';
import * as OrgApi from '../../services/org';
import type { PermissionDto } from '../Permission/data';
import type { UserDto } from './data';

type ListItem = UserDto;

export default (): React.ReactNode => {
    const { initialState } = useModel('@@initialState');
    const [ownedPermRowKeys, setOwnedPermRowKeys] = useState<ReactText[]>([]);
    const [unownedPermRowKeys, setUnOwnedPermRowKeys] = useState<ReactText[]>([]);
    const [ownedRoleRowKeys, setOwnedRoleRowKeys] = useState<ReactText[]>([]);
    const [unownedRoleRowKeys, setUnOwnedRoleRowKeys] = useState<ReactText[]>([]);
    const actionRef = useRef<ActionType>();
    const ownedRoleActionRef = useRef<ProListActionType>();
    const unownedRoleActionRef = useRef<ProListActionType>();
    const ownedPermActionRef = useRef<ProListActionType>();
    const unownedPermActionRef = useRef<ProListActionType>();
    const accessToken = initialState?.currentUser?.accessToken ?? "";

    const ownedPermSelection = {
        selectedRowKeys: ownedPermRowKeys,
        onChange: (keys: ReactText[]) => setOwnedPermRowKeys(keys),
    };
    const unownedPermSelection = {
        selectedRowKeys: unownedPermRowKeys,
        onChange: (keys: ReactText[]) => setUnOwnedPermRowKeys(keys),
    };
    const ownedRoleSelection = {
        selectedRowKeys: ownedRoleRowKeys,
        onChange: (keys: ReactText[]) => setOwnedRoleRowKeys(keys),
    };
    const unownedRoleSelection = {
        selectedRowKeys: unownedRoleRowKeys,
        onChange: (keys: ReactText[]) => setUnOwnedRoleRowKeys(keys),
    };
    const columns: ProColumns<ListItem>[] = [
        {
            title: '用户名',
            dataIndex: 'username',
        },
        {
            title: '工号',
            dataIndex: 'jobNo',
        },
        {
            title: '姓氏',
            hideInSearch: true,
            dataIndex: 'lastName',
        },
        {
            title: '名称',
            hideInSearch: true,
            dataIndex: 'firstName'
        },
        {
            title: '身份证号',
            dataIndex: 'idcard',
        },
        {
            title: '手机号',
            dataIndex: 'phone',
        },
        {
            title: '最近登录 Ip',
            hideInSearch: true,
            dataIndex: 'lastIp',
        },
        {
            title: '最近登录时间',
            hideInSearch: true,
            dataIndex: 'lastLoginAt',
            valueType: 'dateTime',
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
                            await UserApi.updateAsync(accessToken, record.id, value as ListItem);
                            if (actionRef.current) {
                                actionRef.current.reload();
                            }
                            notification.success({ message: "更新用户成功！" });
                            return true;
                        }
                        catch (ex) {
                            notification.error({ message: '更新用户失败！', description: ex.message });
                            return false;
                        }

                        return false;
                    }}
                >
                    <ProForm.Group>
                        <ProFormText
                            width="md"
                            name="username"
                            label="用户名"
                            placeholder="请输入用户名"
                            readonly
                        />
                        <ProFormText
                            width="md"
                            name="phone"
                            label="手机号"
                            placeholder="请输入手机号"
                        />
                    </ProForm.Group>
                    <ProForm.Group>
                        <ProFormText
                            width="md"
                            name="lastName"
                            label="姓氏"
                            placeholder="请输入姓氏"
                        />
                        <ProFormText
                            width="md"
                            name="firstName"
                            label="名称"
                            placeholder="请输入名称"
                        />
                    </ProForm.Group>
                    <ProFormSelect
                        width="md"
                        name="clientId"
                        label="所属组织"
                        request={async () => {
                            try {
                                const orgs = await OrgApi.queryAsync(accessToken, null);
                                if (orgs.data) {
                                    return orgs.data.map(itm => ({ label: itm.name, value: itm.id }));
                                }

                                return [];
                            }
                            catch (ex) {
                                notification.error({ message: '获取组织机构列表时遇到错误！', description: ex.message });
                                return [];
                            }
                        }
                        }
                    />
                    <ProForm.Group>
                        <ProFormText
                            width="md"
                            name="jobNo"
                            label="工号"
                            placeholder="请输入工号"
                        />
                        <ProFormText
                            width="md"
                            name="position"
                            label="职务"
                        />
                    </ProForm.Group>
                    <ProForm.Group>
                        <ProFormText
                            width="md"
                            name="idCard"
                            label="身份证"
                        />
                        <ProFormText
                            width="md"
                            name="homeAddress"
                            label="家庭住址"
                        /></ProForm.Group>
                </DrawerForm>,
                <ModalForm key="roles"
                    submitter={{
                        submitButtonProps: {
                            style: {
                                display: 'none',
                            },
                        },
                    }}
                    trigger={
                        <a>角色管理</a>
                    }>
                    <ProCard >
                        <ProCard bordered>
                            <ProList<API.RoleDto> style={{ maxHeight: '500px', overflow: 'auto' }}
                                actionRef={ownedRoleActionRef}
                                size='small'
                                split={true}
                                headerTitle="已经拥有的角色"
                                rowKey="id"
                                request={async () => {
                                    try {
                                        const result = await UserApi.getRolesAsync(accessToken, record.id);
                                        return result;
                                    }
                                    catch (ex) {
                                        notification.error({ message: '获取已拥有的角色时遇到错误！', description: ex.message });
                                        throw ex;
                                    }
                                }}
                                metas={{
                                    title: {
                                        dataIndex: 'name',
                                        title: '名称'
                                    }
                                }}
                                rowSelection={ownedRoleSelection}
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
                                        await UserApi.removeRolesAsync(accessToken, record.id, ownedRoleRowKeys.map(itm => itm.toString()));
                                        ownedRoleActionRef.current?.reload()
                                        unownedRoleActionRef.current?.reload()
                                        setOwnedRoleRowKeys([]);
                                    }
                                    catch (ex) {
                                        notification.error({ message: '用户角色删除失败！', description: ex.message });
                                    }
                                }}>{'>>'}</Button>
                            </ProCard>
                            <ProCard type="inner" >
                                <Button onClick={async () => {
                                    try {
                                        await UserApi.addRolesAsync(accessToken, record.id, unownedRoleRowKeys.map(itm => itm.toString()));
                                        ownedRoleActionRef.current?.reload()
                                        unownedRoleActionRef.current?.reload()
                                        setUnOwnedRoleRowKeys([]);
                                    }
                                    catch (ex) {
                                        notification.error({ message: '用户角色添加失败！', description: ex.message });
                                    }
                                }}>{'<<'}</Button>
                            </ProCard>
                        </ProCard>
                        <ProCard bordered >
                            <ProList<API.RoleDto> style={{ maxHeight: '500px', overflow: 'auto' }}
                                actionRef={unownedRoleActionRef}
                                size='small'
                                split={true}
                                headerTitle="未拥有的角色"
                                rowKey="id"
                                request={async () => {
                                    try {
                                        const result = await RoleApi.queryAsync({ access_token: accessToken, excludeUserId: record.id });
                                        return result;
                                    }
                                    catch (ex) {
                                        notification.error({ message: "获取未拥有的角色时遇到错误", description: ex.message });
                                        return [];
                                    }
                                }}
                                metas={{
                                    title: {
                                        dataIndex: 'name',
                                        title: '名称'
                                    }
                                }}
                                rowSelection={unownedRoleSelection}
                            />
                        </ProCard>
                    </ProCard>
                </ModalForm>,
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
                                actionRef={ownedPermActionRef}
                                size='small'
                                split={true}
                                headerTitle="已经拥有的权限"
                                rowKey="id"
                                request={async () => {
                                    try {
                                        const result = await UserApi.getPermsAsync(accessToken, record.id);
                                        return result;
                                    }
                                    catch (ex) {
                                        notification.error({ message: '获取已拥有的权限数据失败！', description: ex.message });
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
                                        await UserApi.removePermsAsync(accessToken, record.id, ownedPermRowKeys.map(itm => itm.toString()));
                                        ownedPermActionRef.current?.reload()
                                        unownedPermActionRef.current?.reload()
                                        setOwnedPermRowKeys([]);
                                    }
                                    catch (ex) {
                                        notification.error({ message: '用户权限删除失败！', description: ex.message });
                                    }
                                }}>{'>>'}</Button>
                            </ProCard>
                            <ProCard type="inner" >
                                <Button onClick={async () => {
                                    try {
                                        await UserApi.addPermsAsync(accessToken, record.id, unownedPermRowKeys.map(itm => itm.toString()));
                                        ownedPermActionRef.current?.reload()
                                        unownedPermActionRef.current?.reload()
                                        setUnOwnedPermRowKeys([]);
                                    }
                                    catch (ex) {
                                        notification.error({ message: '用户权限添加失败！', description: ex.message });
                                    }
                                }}>{'<<'}</Button>
                            </ProCard>
                        </ProCard>
                        <ProCard bordered >
                            <ProList<PermissionDto> style={{ maxHeight: '500px', overflow: 'auto' }}
                                actionRef={unownedPermActionRef}
                                size='small'
                                split={true}
                                headerTitle="未拥有的权限"
                                rowKey="id"
                                request={async () => {
                                    try {
                                        const result = await PermissionApi.queryAsync({ access_token: accessToken, excludeUserId: record.id });
                                        return result;
                                    }
                                    catch (ex) {
                                        notification.error({ message: '获取未拥有的权限数据失败！', description: ex.message });
                                        return [];
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
                </ModalForm>,
                <a key='delete' onClick={() => Modal.confirm({
                    title: '操作',
                    content: `确认要删除用户（${record.username}）吗？`,
                    okText: '确认',
                    cancelText: '取消',
                    onOk: async () => {
                        try {
                            await UserApi.deleteAsync(accessToken, record.id);
                            notification.success({ message: `用户删除成功！` });
                            if (actionRef.current) {
                                actionRef.current.reload();
                            }
                        }
                        catch (ex) {
                            notification.error({ message: '删除用户失败！', description: ex.message });
                        }
                    }
                })}>删除</a>,
                <a key='enable' onClick={() => Modal.confirm({
                    title: '操作',
                    content: `确认要${record.isActive ? '冻结' : '激活'}用户（${record.username}）吗？`,
                    okText: '确认',
                    cancelText: '取消',
                    onOk: async () => {
                        try {
                            await UserApi.inActiveAsync(accessToken, record.id, !record.isActive);
                            if (actionRef.current) {
                                actionRef.current.reload();
                            }
                            notification.success({ message: `${record.isActive ? '冻结' : '激活'}用户成功！` });
                            return true;
                        }
                        catch (ex) {
                            notification.error({ message: `${record.isActive ? '冻结' : '激活'}用户失败！`, description: ex.message });
                            return false;
                        }
                    }
                })}>{record.isActive ? '冻结' : '激活'}</a>,
                <a key='resetPwd' onClick={() => Modal.confirm({
                    title: '操作',
                    content: `确认要重置用户（${record.username}）的密码吗？`,
                    okText: '确认',
                    cancelText: '取消',
                    onOk: async () => {
                        try {
                            const pwd = await UserApi.resetPwdAsync(accessToken, record.id);
                            notification.success({ message: `密码重置成功`, description: `新的密码为：${pwd}，请妥善保管。` });
                            return true;
                        }
                        catch (ex) {
                            notification.error({ message: '用户密码重置失败！', description: ex.message });
                            return false;
                        }
                    }
                })}>重置密码</a>

            ],
        }
    ];

    return (
        <PageContainer>
            <ProTable<ListItem>
                rowKey="id"
                columns={columns}
                actionRef={actionRef}
                request={async (params) => {
                    try {
                        const users = await UserApi.queryAsync({
                            access_token: accessToken, firstname: params.firstName, lastname: params.lastName, jobNo: params.jobNo,
                            idCard: params.idCard, phone: params.phone, email: params.email, excludeOrgId: params.excludeOrgId,
                            isActive: params.isActive, pageIndex: params.current, pageSize: params.pageSize
                        });
                        return users;
                    }
                    catch (ex) {
                        notification.error({ message: '获取用户数据失败！', description: ex.message });
                        return [];
                    }
                }}
                toolBarRender={() => [
                    <ModalForm
                        width="740px"
                        title="新建用户"
                        trigger={
                            <Button type="primary">
                                <PlusOutlined />
                                新建用户
                            </Button>
                        }
                        onFinish={async (value) => {
                            try {
                                await UserApi.createAsync(accessToken, value as ListItem);
                                if (actionRef.current) {
                                    actionRef.current.reload();
                                }
                                notification.success({ message: "创建用户成功！" });
                                return true;
                            }
                            catch (ex) {
                                notification.error({ message: '创建用户失败！', description: ex.message });
                                return false;
                            }
                        }}
                    >
                        <ProForm.Group>
                            <ProFormText
                                width="md"
                                name="username"
                                label="用户名"
                                placeholder="请输入用户名"
                                rules={[
                                    {
                                        required: true,
                                        message: '用户名不能为空',
                                    }
                                ]}
                            />
                            <ProFormText.Password
                                width="md"
                                name="password"
                                label="初始密码"
                                placeholder="请输入初始密码"
                            />
                        </ProForm.Group>
                        <ProForm.Group>
                            <ProFormText
                                width="md"
                                name="lastName"
                                label="姓氏"
                                placeholder="请输入姓氏"
                            />
                            <ProFormText
                                width="md"
                                name="firstName"
                                label="名称"
                                placeholder="请输入名称"
                            />
                        </ProForm.Group>
                        <ProForm.Group>
                            <ProFormText
                                width="md"
                                name="phone"
                                label="手机号"
                                placeholder="请输入手机号"
                            />
                            <ProFormSelect
                                width="md"
                                name="clientId"
                                label="所属组织"
                                request={async () => {
                                    try {
                                        const orgs = await OrgApi.queryAsync(accessToken, null);
                                        if (orgs.data) {
                                            return orgs.data.map(itm => ({ label: itm.name, value: itm.id }));
                                        }

                                        return [];
                                    }
                                    catch (ex) {
                                        notification.error({ message: '获取组织机构列表时遇到错误！', description: ex.message });
                                        return [];
                                    }
                                }
                                }
                            />
                        </ProForm.Group>
                        <ProForm.Group>
                            <ProFormText
                                width="md"
                                name="jobNo"
                                label="工号"
                                placeholder="请输入工号"
                            />
                            <ProFormText
                                width="md"
                                name="position"
                                label="职务"
                            />
                        </ProForm.Group>
                        <ProForm.Group>
                            <ProFormText
                                width="md"
                                name="idCard"
                                label="身份证"
                            />
                            <ProFormText
                                width="md"
                                name="homeAddress"
                                label="家庭住址"
                            /></ProForm.Group>
                    </ModalForm>
                ]}
            />

        </PageContainer>
    )
};
