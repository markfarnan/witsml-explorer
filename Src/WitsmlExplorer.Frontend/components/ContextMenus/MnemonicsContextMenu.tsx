import { Typography } from "@equinor/eds-core-react";
import { ListItemIcon, MenuItem } from "@material-ui/core";
import React from "react";
import { DisplayModalAction, HideContextMenuAction, HideModalAction } from "../../contexts/operationStateReducer";
import OperationType from "../../contexts/operationType";
import { DeleteLogCurveValuesJob } from "../../models/jobs/deleteLogCurveValuesJob";
import JobService, { JobType } from "../../services/jobService";
import { colors } from "../../styles/Colors";
import Icon from "../../styles/Icons";
import ConfirmModal from "../Modals/ConfirmModal";
import ContextMenu from "./ContextMenu";

export interface MnemonicsContextMenuProps {
  dispatchOperation: (action: HideModalAction | HideContextMenuAction | DisplayModalAction) => void;
  deleteLogCurveValuesJob: DeleteLogCurveValuesJob;
}

const MnemonicsContextMenu = (props: MnemonicsContextMenuProps): React.ReactElement => {
  const { deleteLogCurveValuesJob, dispatchOperation } = props;

  const deleteLogCurveValues = async () => {
    dispatchOperation({ type: OperationType.HideModal });
    await JobService.orderJob(JobType.DeleteCurveValues, deleteLogCurveValuesJob);
  };

  const onClickDelete = async () => {
    const confirmation = (
      <ConfirmModal
        heading={"Delete values for logcurve?"}
        content={getContentMessage(deleteLogCurveValuesJob)}
        onConfirm={deleteLogCurveValues}
        confirmColor={"danger"}
        switchButtonPlaces={true}
      />
    );
    dispatchOperation({ type: OperationType.DisplayModal, payload: confirmation });
  };

  return (
    <ContextMenu
      menuItems={[
        <MenuItem key={"delete"} onClick={onClickDelete}>
          <ListItemIcon>
            <Icon name="deleteToTrash" color={colors.interactive.primaryResting} />
          </ListItemIcon>
          <Typography color={"primary"}>Delete</Typography>
        </MenuItem>
      ]}
    />
  );
};

const getContentMessage = (deleteLogCurveValuesJob: DeleteLogCurveValuesJob) => {
  const mnemonics = deleteLogCurveValuesJob.mnemonics.map((m) => `"${m}"`).join(", ");
  return (
    <>
      <p>This will permanently delete selected index ranges: </p>
      {deleteLogCurveValuesJob.indexRanges.map((r) => (
        <p key={r.startIndex}>
          [{r.startIndex} - {r.endIndex}]
          <br />
        </p>
      ))}
      <p>
        from mnemonics: <br />
        {mnemonics}
      </p>
    </>
  );
};

export default MnemonicsContextMenu;
